import { defineStore } from "pinia";

export const useLocalStore = defineStore("localStore", {
  state: () => ({
    dark: "auto",
    miniEnable: false,
  }),
  persist: {
    enabled: true,
    strategies: [
      {
        storage: localStorage,
      },
    ],
  },
});
