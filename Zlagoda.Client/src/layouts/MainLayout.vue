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

        <q-item to="/test" exact v-ripple>
          <q-item-section avatar>
            <q-icon :name="fasFlask" />
          </q-item-section>
          <q-item-section avatar>
            <q-item-label>Test</q-item-label>
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
import { ref, watch } from "vue";
import { useQuasar } from "quasar";
import { useLocalStore } from "stores/localStore";
import { storeToRefs } from "pinia";
import {
  fasBars,
  fasHouse,
  fasAngleUp,
  fasAngleLeft,
  fasSun,
  fasMoon,
  fasFlask,
} from "@quasar/extras/fontawesome-v6";

defineOptions({
  name: "MainLayout",
});

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