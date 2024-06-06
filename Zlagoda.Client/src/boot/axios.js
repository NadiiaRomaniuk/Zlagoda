import { boot } from "quasar/wrappers";
import axios from "axios";
//import { useLocalStore } from "stores/localStore";
//import { storeToRefs } from "pinia";
//import { useAuth } from "vue-auth3";

// Be careful when using SSR for cross-request state pollution
// due to creating a Singleton instance here;
// If any client changes this (global) instance, it might be a
// good idea to move this instance creation inside of the
// "export default () => {}" function below (which runs individually
// for each client)
const api = axios.create({ baseURL: "/api" });

export default boot(({ app }) => {
  //  const localStore = useLocalStore(store);
  //  const { token } = storeToRefs(localStore);
  const auth = app.config.globalProperties.$auth;

  console.log("auth", auth.token());

  api.interceptors.request.use((config) => {
    if (auth && auth.token() !== null)
      config.headers["Authorization"] = `Bearer ${auth.token()}`;
    return config;
  });
  // for use inside Vue files (Options API) through this.$axios and this.$api

  app.config.globalProperties.$axios = axios;
  // ^ ^ ^ this will allow you to use this.$axios (for Vue Options API form)
  //       so you won't necessarily have to import axios in each vue file

  app.config.globalProperties.$api = api;
  // ^ ^ ^ this will allow you to use this.$api (for Vue Options API form)
  //       so you can easily perform requests against your app's API
});

export { api };
