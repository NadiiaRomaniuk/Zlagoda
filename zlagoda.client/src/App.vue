<template>
  <router-view />
</template>

<script setup>
import { useQuasar } from "quasar";
import { useLocalStore } from "stores/localStore";

const $q = useQuasar();
const localStore = useLocalStore();

defineOptions({
  name: "App",
});

if ($q.dark.mode !== localStore.dark) $q.dark.set(localStore.dark);
</script>
