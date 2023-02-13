let sender = document.getElementById("senderEmail");
let receiver = document.getElementById("receiverEmail");
let message = document.getElementById("chatMessage");
let sendMessageBtn = document.getElementById("sendMessage");
let messagesList = document.getElementById("messagesList");

let connectionBasicChat = new signalR.HubConnectionBuilder()
  .withAutomaticReconnect()
  .withUrl("/hubs/basicChat", signalR.HttpTransportType.WebSockets)
  .build();

sendMessageBtn.disabled = true;

sendMessageBtn.addEventListener("click", function (event) {
  if (receiver.value.length > 0) {
    connectionBasicChat
      .send(
        "SendMessageToReciever",
        sender.value,
        receiver.value,
        message.value
      )
      .catch((err) => console.log(err.toString()));
  } else {
    connectionBasicChat
      .send("SendMessageToAll", sender.value, message.value)
      .catch((err) => console.log(err.toString()));
  }

  event.preventDefault();
});

connectionBasicChat.on("newMessageRecieved", (user, newMessage) => {
  let li = document.createElement("li");
  messagesList.appendChild(li);
  li.textContent = `${user} - ${newMessage} `;
});

connectionBasicChat.start().then(() => {
  sendMessageBtn.disabled = false;
});
