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
        path: "/employees",
        component: () => import("pages/EmployeesPage.vue"),
        meta: { auth: "manager" },
      },
      {
        path: "/category",
        component: () => import("pages/CategoryPage.vue"),
        meta: { auth: "manager" },
      },
      {
        path: "/product",
        component: () => import("pages/ProductPage.vue"),
        meta: { auth: "manager" },
      },
      {
        path: "/forbidden",
        component: () => import("pages/ErrorForbidden.vue"),
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
