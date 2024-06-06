<template>
  <q-layout view="hhh LpR fff">
    <q-header>
      <q-toolbar>
        <q-btn
          flat
          dense
          round
          :icon="fasBars"
          aria-label="Menu"
          @click="toggleLeftDrawer"
        />

        <q-toolbar-title> Zlagoda </q-toolbar-title>

        <span>{{ auth.remember() }}</span>

        <q-toggle
          v-model="darkSkin"
          :unchecked-icon="fasSun"
          :checked-icon="fasMoon"
          color="black"
          keep-color
        />
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="drawerOpen"
      show-if-above
      bordered
      :mini-to-overlay="miniEnable"
      :mini="miniEnable && miniState"
      class="drawer"
      :width="200"
      @mouseover="miniState = false"
      @mouseout="miniState = true"
    >
      <q-list>
        <q-item to="/" exact v-ripple>
          <q-item-section avatar>
            <q-icon :name="fasHouse" />
          </q-item-section>
          <q-item-section avatar>
            <q-item-label>Home</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/login" exact v-ripple v-if="!auth.check()">
          <q-item-section avatar>
            <q-icon :name="fasRightToBracket" />
          </q-item-section>
          <q-item-section avatar>
            <q-item-label>Login</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/test" exact v-ripple v-if="auth.check()">
          <q-item-section avatar>
            <q-icon :name="fasFlask" />
          </q-item-section>
          <q-item-section avatar>
            <q-item-label>Test</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/manager" exact v-ripple v-if="auth.check(['Manager'])">
          <q-item-section avatar>
            <q-icon :name="fasBriefcase" />
          </q-item-section>
          <q-item-section avatar>
            <q-item-label>Manager</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/cashier" exact v-ripple v-if="auth.check('Cashier')">
          <q-item-section avatar>
            <q-icon :name="fasCashRegister" />
          </q-item-section>
          <q-item-section avatar>
            <q-item-label>Cashier</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable="" v-ripple v-if="auth.check()" @click="logout">
          <q-item-section avatar>
            <q-icon :name="fasRightFromBracket" />
          </q-item-section>
          <q-item-section avatar>
            <q-item-label>Logout</q-item-label>
          </q-item-section>
        </q-item>
      </q-list>
      <q-btn
        flat
        @click="miniChange"
        v-if="!$q.platform.is.mobile"
        align="right"
        :icon="fasAngleLeft"
        class="mini-button full-width absolute-bottom-left"
        :class="miniEnable ? 'mini-button--rotated' : ''"
      />
    </q-drawer>

    <q-page-container>
      <router-view />
      <q-page-scroller
        position="bottom-right"
        :scroll-offset="150"
        :offset="$q.screen.name === 'xs' ? [9, 9] : [18, 18]"
      >
        <q-btn fab-mini :icon="fasAngleUp" color="accent" />
      </q-page-scroller>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, watch, computed } from "vue";
import { useQuasar, Notify } from "quasar";
import { useLocalStore } from "stores/localStore";
import { storeToRefs } from "pinia";
//import { auth } from "boot/auth";
import { useAuth } from "vue-auth3";
import {
  fasBars,
  fasHouse,
  fasAngleUp,
  fasAngleLeft,
  fasSun,
  fasMoon,
  fasFlask,
  fasCashRegister,
  fasBriefcase,
  fasRightToBracket,
  fasRightFromBracket,
} from "@quasar/extras/fontawesome-v6";

defineOptions({
  name: "MainLayout",
});

const auth = useAuth();
const $q = useQuasar();
const miniState = ref(true);
const localStore = useLocalStore();
const { dark, miniEnable } = storeToRefs(localStore);
const drawerOpen = ref(false);
const darkSkin = ref($q.dark.isActive);

const toggleLeftDrawer = () => (drawerOpen.value = !drawerOpen.value);
const miniChange = () =>
  (miniEnable.value =
    miniEnable.value === undefined ? true : !miniEnable.value);

const logout = () => {
  $q.dialog({
    title: "Logout",
    class: "q-pa-sm",
    message: "Are you sure?",
    ok: {
      color: "positive",
      noCaps: true,
      label: "OK",
    },
    cancel: {
      color: "negative",
      noCaps: true,
      label: "Cancel",
    },
    persistent: true,
    transitionShow: "flip-down",
    transitionHide: "flip-up",
  }).onOk(() => {
    Notify.create("Logged out");
    auth.logout({
      makeRequest: false,
    });
    auth.unremember();
  });
};

watch(darkSkin, () => {
  dark.value = darkSkin.value;
  $q.dark.set(darkSkin.value);
});
</script>

<style lang="sass">
.mini-button .q-icon
  position: relative
  transition: transform .3s

.mini-button--rotated .q-icon
  transform: rotate(180deg)

.drawer
  background-color: #eee
  .q-dark &
    background-color: #222
</style>
