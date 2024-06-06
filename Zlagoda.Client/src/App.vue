<template>
  <router-view v-if="auth.ready()" />
  <div class="window-height flex flex-center" v-else>
    <q-spinner-dots color="primary" size="2em" />
  </div>
</template>

<script setup>
import { useQuasar } from "quasar";
import { useLocalStore } from "stores/localStore";
import { useAuth } from "vue-auth3";

const auth = useAuth();
const $q = useQuasar();
const localStore = useLocalStore();

defineOptions({
  name: "App",
});

const updateUser = (token) => {
  if (token) {
    const payload = JSON.parse(
      atob(token.substring(token.indexOf(".") + 1, token.lastIndexOf(".")))
    );
    auth.remember(payload.uid);
    auth.user({
      roles: [payload.roles],
      login: payload.uid,
    });
  }
};

if (auth.token()) auth.refresh();
updateUser(auth.token());

if ($q.dark.mode !== localStore.dark) $q.dark.set(localStore.dark);
</script>
