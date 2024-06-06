const routes = [
  {
    path: "/",
    component: () => import("layouts/MainLayout.vue"),
    children: [
      {
        path: "",
        component: () => import("pages/IndexPage.vue"),
        meta: { auth: undefined },
      },
      {
        path: "/login",
        component: () => import("pages/LoginPage.vue"),
        meta: { auth: false },
      },
      {
        path: "/test",
        component: () => import("pages/TestPage.vue"),
        meta: { auth: true },
      },
      {
        path: "/manager",
        component: () => import("pages/ManagerPage.vue"),
        meta: { auth: ["Manager"] },
      },
      {
        path: "/cashier",
        component: () => import("pages/CashierPage.vue"),
        meta: { auth: ["Cashier"] },
      },
    ],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: "/:catchAll(.*)*",
    component: () => import("pages/ErrorNotFound.vue"),
  },
];

export default routes;
