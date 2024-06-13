<template>
  <q-page class="full-height column flex-center" padding>
    <div class="text-h5 q-mt-md q-ml-md absolute-top absolute-left">
      <q-icon class="q-mx-sm" :name="fasRightToBracket" />Login
    </div>
    <transition
      appear
      enter-active-class="animated flipInX"
      leave-active-class="animated flipOutX"
    >
      <q-card
        class="q-mb-xl"
        style="min-width: 380px"
        :class="shake ? 'animated shake' : ''"
      >
        <q-card-section class="q-pa-xl">
          <q-form @submit="login" :disable="wait">
            <q-input v-model="username" label="Login" :disable="wait" autofocus>
              <template v-slot:prepend>
                <q-icon :name="fasEnvelope"></q-icon>
              </template>
            </q-input>
            <q-input
              v-model="password"
              :type="isPwd ? 'password' : 'text'"
              ref="input"
              :label="'Password'"
              :disable="wait"
            >
              <template v-slot:prepend>
                <q-icon :name="fasLock"></q-icon>
              </template>
              <template v-slot:append>
                <q-icon
                  class="cursor-pointer"
                  :name="isPwd ? fasEye : fasEyeSlash"
                  @click="isPwd = !isPwd"
                ></q-icon>
              </template>
            </q-input>
            <div class="row q-mt-xl">
              <div class="col">
                <q-checkbox
                  v-model="remember"
                  :label="'RememberMe'"
                  :disable="wait"
                  style="margin-left: -9px"
                ></q-checkbox>
              </div>
              <div class="col">
                <q-btn
                  class="float-right"
                  :label="'Login'"
                  type="submit"
                  color="primary"
                  :icon="fasKey"
                  :loading="wait"
                  :disable="wait"
                  no-caps
                >
                  <template v-slot:loading>
                    <q-spinner class="on-left"></q-spinner>Login
                  </template>
                </q-btn>
              </div>
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </transition>
  </q-page>
</template>

<script setup>
import { ref, nextTick } from "vue";
import notify from "../notices";
import { useAuth } from "vue-auth3";
import {
  fasRightToBracket,
  fasEnvelope,
  fasLock,
  fasEye,
  fasEyeSlash,
  fasKey,
} from "@quasar/extras/fontawesome-v6";

const auth = useAuth();
const username = ref("");
const password = ref("");
const isPwd = ref(true);
const remember = ref(true);
const wait = ref(false);
const shake = ref(false);
const input = ref(null);

defineOptions({
  name: "LoginPage",
});

const login = () => {
  wait.value = true;
  void auth
    .login({
      data: { Login: username.value, Password: password.value },
      staySignedIn: remember.value,
      redirect: "/",
      fetchUser: false,
    })
    .then(
      () => {
        auth.remember(auth.user().name);
      },
      (error) => {
        if (error.request?.status === 401)
          notify.error("Incorrect login or password");
        else notify.error(`Login error ${error.request?.result}`);
        console.log("Incorrect password", error);
        shake.value = true;
        wait.value = false;
        void nextTick(() => {
          input?.value?.focus();
        });
        setTimeout(() => {
          shake.value = false;
        }, 1500);
      }
    )
    .then(() => (wait.value = false));
};
</script>
