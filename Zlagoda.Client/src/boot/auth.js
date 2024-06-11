import { createAuth } from "vue-auth3";
import { boot } from "quasar/wrappers";
import { api } from "boot/axios";
import driverAuthBearer from "vue-auth3/drivers/auth/bearer";

export default boot(({ app, router, store }) => {
  const auth = createAuth({
    stores: ["storage"],
    forbiddenRedirect: "/forbidden",
    notFoundRedirect: "/",
    fetchData: {
      enabled: false,
    },
    refreshToken: {
      enabled: true,
      interval: 50,
    },
    plugins: {
      router,
    },
    drivers: {
      http: {
        request: api,
      },
      auth: driverAuthBearer,
    },
  });

  app.use(auth);
});
