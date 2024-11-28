const stompit = require('stompit');
const { getMaxListeners } = require('stompit/lib/IncomingFrameStream');


const connectOptions = {
  host: 'localhost',
  port: 61613,
  connectHeaders: {
    host: '/',
    login: 'admin',
    passcode: 'admin',
    'heart-beat': '5000,5000'
  }
};

stompit.connect(connectOptions, (error, client) => {
  if (error) {
    console.log('connect error ' + error.message);
    return;
  }

  const subscribeHeaders = {
    destination: '/topic/tropico',
    ack: 'client-individual'
  };
  client.setMaxListeners(2000);
  client.subscribe(subscribeHeaders, (error, message) => {
    if (error) {
      console.log('subscribe error ' + error.message);
      return;
    }

    const readMessage = (message) => {
      message.readString('utf-8', (error, body) => {
        if (error) {
          console.log('read message error ' + error.message);
          return;
        }
        console.log('received message: ' + body);
        client.ack(message);
      });
    };

    // Start reading messages
    readMessage(message);

    // Continue reading messages
    client.on('message', readMessage);
  });
});