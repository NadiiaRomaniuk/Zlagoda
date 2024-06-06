import { createAuth } from "vue-auth3";
import { boot } from "quasar/wrappers";
import { api } from "boot/axios";
import driverAuthBearer from "vue-auth3/drivers/auth/bearer";
import driverHttpAxios from "vue-auth3/drivers/http/axios";

export default boot(({ app, router, store }) => {
  const auth = createAuth({
    //initSync: true,
    stores: ["storage"],
    rolesKey: "roles",
    notFoundRedirect: "/",
    staySignedIn: true,
    fetchData: {
      enabled: false,
      cache: true,
    },
    refreshToken: {
      enabled: true,
    },
    plugins: {
      router,
    },
    drivers: {
      http: {
        request: api,
        // request: app.config.globalProperties.$api,
      },
      //http: driverHttpAxios,
      auth: driverAuthBearer,
    },
  });

  app.use(auth);
});
