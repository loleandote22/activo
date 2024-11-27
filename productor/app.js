const express = require('express');
const stompit = require('stompit');
const swaggerJsdoc = require('swagger-jsdoc');
const swaggerUi = require('swagger-ui-express');
const app = express();
const port = 3000;

// Middleware para parsear el cuerpo de las solicitudes como JSON
app.use(express.json());

// Configuración de la conexión a ActiveMQ
const connectOptions = {
  host: 'activemq', 
  port: 61613, 
  connectHeaders: {
    host: '/',
    login: 'admin', 
    passcode: 'admin', 
  }
};

// Configuración de Swagger
const swaggerOptions = {
  swaggerDefinition: {
    openapi: '3.0.0',
    info: {
      title: 'API de ejemplo',
      version: '1.0.0',
      description: 'Documentación de la API de ejemplo con Swagger',
    },
    servers: [
      {
        url: `http://localhost:${port}`,
      },
    ],
  },
  apis: ['./app.js'], // Archivos que contienen anotaciones Swagger
};

const swaggerDocs = swaggerJsdoc(swaggerOptions);
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerDocs));

/**
 * @swagger
 * /endpoint/{topic}:
 *   post:
 *     summary: Envía datos a ActiveMQ
 *     parameters:
 *       - in: path
 *         name: topic
 *         required: true
 *         schema:
 *           type: string
 *         description: El tópico al que se enviarán los datos
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               message:
 *                 type: string
 *                 example: hola buenas
 *     responses:
 *       200:
 *         description: Datos recibidos y enviados a ActiveMQ
 *       500:
 *         description: Error de conexión a ActiveMQ
 */
app.post('/endpoint/:topic', (req, res) => {
  const data = req.body;
  const topic = req.params.topic;

  // Conectar a ActiveMQ y enviar el mensaje
  stompit.connect(connectOptions, (error, client) => {
    if (error) {
      console.error('Error de conexión a ActiveMQ:', error);
      res.status(500).send('Error de conexión a ActiveMQ');
      return;
    }

    const frame = client.send({
      destination: `/topic/${topic}`, // Usa el parámetro de la URL como destino
    });
    frame.write(JSON.stringify(data));
    frame.end();

    client.disconnect();

    res.send('Datos recibidos y enviados a ActiveMQ');
  });
});

app.listen(port, () => {
  console.log(`Servidor escuchando en http://localhost:${port}`);
});