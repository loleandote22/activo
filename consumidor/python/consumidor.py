import stomp

class ChatConsumer(stomp.ConnectionListener):
    def __init__(self, conn):
        self.conn = conn

    def on_message(self, frame, message):
        print(f"Mensaje recibido: {message}")

# Configuración
host = 'localhost'
port = 61613
topic = '/queue/tropico'
print("HOla ya estoy aqui")
conn = stomp.Connection([(host, port)])
conn.set_listener('', ChatConsumer(conn))
conn.start()
conn.connect(wait=True)

# Suscribirse al tópico
conn.subscribe(destination=topic, id=1, ack='auto')

print("Escuchando mensajes... Presiona Ctrl+C para salir.")

# Mantener el programa activo
i = 0
try:
    while True:
        i += 1
except KeyboardInterrupt:
    print("\nDesconectando...")
    conn.disconnect()
