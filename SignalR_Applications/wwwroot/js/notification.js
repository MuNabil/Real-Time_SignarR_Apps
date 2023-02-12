let notificationInput = document.getElementById("notificationInput");
let sendButton = document.getElementById("sendButton");
let messageList = document.getElementById("messageList");
let notificationCounter = document.getElementById("notificationCounter");

//create connection
var connectionNotification = new signalR.HubConnectionBuilder()
  .withAutomaticReconnect()
  .withUrl("/hubs/notification")
  .build();

// To disable the sendButton untill the hub connection starts successfuly
sendButton.disabled = true;

// When submit buttom click send the messsage to the hub then clear the input field
sendButton.addEventListener("click", function (event) {
  connectionNotification
    .send("SendNotification", notificationInput.value)
    .then(function () {
      notificationInput.value = "";
    });
  event.preventDefault();
});

// To listen to the method that send all messages whenever new message'Notificaation' send.
connectionNotification.on("newMessageLoaded", (messages) => {
  messageList.innerHTML = "";
  notificationCounter.innerHTML =
    "<span>(" + messages.length.toString() + ")</span>";
  for (let i = messages.length - 1; i >= 0; i--) {
    var li = document.createElement("li");
    li.textContent = "Notification - " + messages[i];
    messageList.appendChild(li);
  }
});

//start connection
connectionNotification.start().then(() => {
  connectionNotification.send("SendAllMessages");
  sendButton.disabled = false;
});
