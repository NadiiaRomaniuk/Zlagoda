<template>
  <q-page class="column flex-center">
    <h4>Zlagoda start page</h4>
    <img src="/images/index-cat.jpeg" width="80%" />
    <p>{{ testResult }}</p>
  </q-page>
</template>

<script setup>
import { ref } from "vue";
import { api } from "boot/axios";

defineOptions({
  name: "IndexPage",
});

const testResult = ref("Wait...");

const getTest = () => {
  api
    .get("test")
    .then((result) => {
      testResult.value = result.data;
    })
    .catch((error) => {
      testResult.value = error.message;
      console.error(error.message);
    });
};

getTest();
</script>
