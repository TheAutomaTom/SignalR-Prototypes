<template>
  <h3>SignalR Client Prototype</h3>
  <h3>Vue3</h3>
  <h5><em>In this demo, the server will invert the message a user sends.</em></h5>
  <hr/>
  <br/>

  <div class="table2x3">

    <span>Send:</span>    
    <input 
      type="text" 
      v-model="userInput"
      style="margin-left:1em"
    >
    
    <span>Reply:</span>    
    <span style="margin-left:1em">
      {{ serverReply }}
    </span>
  
    <button 
      @click="sendToServer()"
      class="my-button"
    >Send input
    </button>

  </div>
  
  <br/>
  <hr/>
  <h5><em>Opening a second window and navigating to the same url will show all clients will be updated by the server's reply.</em></h5>
 
</template>

<script setup lang="ts">
import * as signalR from "@microsoft/signalr";
import HmacSHA256 from "crypto-js/hmac-sha256";
import Base64 from "crypto-js/enc-base64";
import Utf8 from "crypto-js/enc-utf8";
import { ref } from "vue";


const userInput = ref("Input...");
const serverReply = ref("...Reply");

console.log("[Client.SignalR] Building connection...");
const connection = new signalR.HubConnectionBuilder()
  //.withUrl("http://localhost:5192/chatHub") //SignalR-POC.ASP
  .withUrl("http://localhost:7230/api", //Serverless running locally
      { accessTokenFactory: () =>  generateAccessToken("Tom") }
    )
  .configureLogging(signalR.LogLevel.Information)
  .withAutomaticReconnect()
  .build();
    
// Inbound: `connection.on()`s should be registered before `.start` executes
connection.on("ReceiveMessage", (reply) => {
  console.log("[Client.SignalR.ReceiveMessage] " + reply);
  serverReply.value = reply;
});

async function start() {
    console.log("[Client.SignalR.start] starting connection...");
  try {
    await connection.start();
    console.log("[Client.SignalR.start] Connected.");
  } catch (err) {
    console.log(err);
    setTimeout(start, 5000);
  }    
};
  
// Outbound
async function sendToServer(){
  try {
    console.log("[Client.SignalR.SendMessage] " + userInput.value);
    await connection.invoke("SendMessage", userInput.value);
  } catch (err) {
    console.error(err);
  }
}


connection.onclose( async () => {
  console.log("[Client.SignalR.onclose] Closed.");
  await start() 
});


// Start the connection.
start();




// this function should be in auth server, do not expose your secret
function generateAccessToken(userName: string) {
    var header = {
      "alg": "HS256",
      "typ": "JWT"
    };

    var stringifiedHeader = Utf8.parse(JSON.stringify(header));
    var encodedHeader =  Base64.stringify(stringifiedHeader); // base64url(stringifiedHeader);

    // customize your JWT token payload here
    var data = {
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": userName,
      "exp": 1699819025,
      'admin': true //isAdmin
    };

    var stringifiedData = Utf8.parse(JSON.stringify(data));
    var encodedData = Base64.stringify(stringifiedData);

    var token = encodedHeader + "." + encodedData;

    var secret = "myFunctionAuthTest"; // do not expose your secret here

    var signature = HmacSHA256(token, secret);
    //signature = base64url(signature);

    var signedToken = token + "." + signature;

    return signedToken;
  }
</script>
<style>
.table2x3{
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  grid-template-rows: repeat(3, 1fr);
}
.my-button{
  grid-column-end: span 2; 
  margin-top:0.5em;
}
</style>