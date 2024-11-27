import stomp

class ChatConsumer(stomp.ConnectionListener):
    def __init__(self, conn):
        self.conn = conn

    def on_message(self, frame, message):
        print(f"Mensaje recibido: {message}")

# Configuración
host = 'activemq'
port = 61613
topic = '/topic/tropico'

conn = stomp.Connection([(host, port)])
conn.set_listener('', ChatConsumer(conn))
conn.start()
conn.connect(wait=True)

# Suscribirse al tópico
conn.subscribe(destination=topic, id=1, ack='auto')

print("Escuchando mensajes... Presiona Ctrl+C para salir.")

# Mantener el programa activo
try:
    while True:
        pass
except KeyboardInterrupt:
    print("\nDesconectando...")
    conn.disconnect()
