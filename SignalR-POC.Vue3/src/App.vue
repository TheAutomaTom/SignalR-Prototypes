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
import * as signalR from "@microsoft/signalr"
import { ref } from "vue";

const userInput = ref("Input...");
const serverReply = ref("...Reply");

console.log("[SignalR] Initializing connection...");
const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5192/chatHub") //SignalR-POC.ASP
  .configureLogging(signalR.LogLevel.Information)
  .withAutomaticReconnect()
  .build();
    
// Inbound: `connection.on()`s should be registered before `.start` executes
connection.on("ReceiveMessage", (reply) => {
  console.log("[SignalR.ReceiveMessage] " + reply);
  serverReply.value = reply;
});

async function start() {
  try {
    await connection.start();
    console.log("[SignalR.start] Connected.");
  } catch (err) {
    console.log(err);
    setTimeout(start, 5000);
  }    
};
  
// Outbound
async function sendToServer(){
  try {
    console.log("[SignalR.SendMessage] " + userInput.value);
    await connection.invoke("SendMessage", userInput.value);
  } catch (err) {
    console.error(err);
  }
}


connection.onclose( async () => {
  console.log("[SignalR.onclose] Closed.");
  await start() 
});


// Start the connection.
start();
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