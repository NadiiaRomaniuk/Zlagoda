<template>
  <q-page padding>
    <div class="text-h5 q-mb-lg">
      <q-icon class="q-mr-md" :name="fasUserGroup" />Employees
    </div>

    <div class="row">
      <div class="col-12 col-sm-10 col-lg-8">
        <q-table
          separator="horizontal"
          :rows="users"
          :columns="columns"
          :filter="filter"
          :filter-method="filterMethod"
          :loading="loading"
          :pagination="pagination"
          binary-state-sort
          no-data-label="No records exists"
          no-results-label="No records found"
          row-key="employeeId"
        >
          <template v-slot:top>
            <div class="row full-width">
              <q-select
                class="col-6 q-pa-sm"
                v-model="filter.role"
                :options="roles"
                label="Role"
                dense
                options-dense
                emit-value
                map-options
                clearable
                :disable="loading || user !== null"
              />
              <q-input
                class="col-6 q-pa-sm"
                dense
                clearable
                debounce="300"
                placeholder="Filter"
                v-model="filter.filter"
                :disable="loading || user !== null"
              >
                <template v-slot:append>
                  <q-icon :name="fasFilter" />
                </template>
              </q-input>
            </div>
          </template>
          <template v-slot:body="props">
            <q-tr
              class="cursor-pointer"
              :props="props"
              @click="startEdit(props.row.employeeId)"
            >
              <template v-for="col in props.cols" :key="col.name">
                <q-td v-if="col.name !== 'role'">{{
                  props.row[col.name]
                }}</q-td>
                <q-td v-else>{{ roleLabel(props.row.role) }}</q-td>
              </template>
            </q-tr>
          </template>
        </q-table>

        <q-page-sticky
          position="bottom-right"
          :offset="$q.screen.name === 'xs' ? [9, 9] : [18, 18]"
        >
          <q-btn
            color="accent"
            :disable="loading || user !== null"
            :icon="fasPlus"
            @click="newUser"
            fab-mini="fab-mini"
          ></q-btn>
        </q-page-sticky>
      </div>
    </div>

    <q-dialog v-model="dlg" persistent v-if="user !== null">
      <q-card
        class="q-dialog-plugin q-pa-md"
        style="width: 800px; max-width: 80vw"
        @click="animateOnEdit"
      >
        <q-card-section>
          <div class="text-h6">
            {{ isNewUser ? "New employee" : `Employee ${user.employeeId}` }}
          </div>
        </q-card-section>
        <q-card-section>
          <div class="row q-col-gutter-x-md">
            <div class="col-12 col-sm-4 col-lg-4">
              <q-input
                v-model="user.surname"
                label="Surname"
                dense
                :rules="[(v) => v.length > 0 || 'Required']"
              ></q-input>
            </div>
            <div class="col-12 col-sm-4 col-lg-4">
              <q-input
                v-model="user.name"
                label="Name"
                dense
                :rules="[(v) => v.length > 0 || 'Required']"
              ></q-input>
            </div>
            <div class="col-12 col-sm-4 col-lg-4">
              <q-input
                v-model="user.patronymic"
                label="Patronymic"
                dense
                lazy-rules
              ></q-input>
            </div>
            <div class="col-12 col-sm-3 col-lg-3">
              <q-input
                v-model="user.city"
                label="City"
                dense
                :rules="[(v) => v.length > 0 || 'Required']"
              ></q-input>
            </div>
            <div class="col-12 col-sm-3 col-lg-3">
              <q-input
                v-model="user.street"
                label="Street"
                dense
                :rules="[(v) => v.length > 0 || 'Required']"
              ></q-input>
            </div>
            <div class="col-12 col-sm-3 col-lg-3">
              <q-input
                v-model="user.zipCode"
                label="ZIP code"
                dense
                :rules="[(v) => v.length > 0 || 'Required']"
              ></q-input>
            </div>
            <div class="col-12 col-sm-3 col-lg-3">
              <q-input
                v-model="user.phoneNumber"
                label="Phone"
                dense
                :rules="[(v) => v.length > 0 || 'Required']"
              ></q-input>
            </div>
            <div class="col-12 col-sm-4 col-lg-4">
              <q-input
                v-model.number="user.salary"
                label="Salary"
                dense
                :rules="[(v) => v > 0 || 'Required']"
              ></q-input>
            </div>
            <div class="col-12 col-sm-4 col-lg-4">
              <q-input
                dense
                v-model="user.dateOfBirth"
                label="Birth date"
                :rules="[(v) => dateRule(v) || 'Date format YYYY-MM-DD']"
              >
                <template v-slot:prepend>
                  <q-icon :name="fasCalendarDay" class="cursor-pointer">
                    <q-popup-proxy
                      transition-show="scale"
                      transition-hide="scale"
                    >
                      <q-date
                        v-model="user.dateOfBirth"
                        mask="YYYY-MM-DD"
                      ></q-date>
                    </q-popup-proxy>
                  </q-icon>
                </template>
              </q-input>
            </div>
            <div class="col-12 col-sm-4 col-lg-4">
              <q-input
                dense
                v-model="user.dateOfStart"
                label="Start date"
                :rules="[(v) => dateRule(v) || 'Date format YYYY-MM-DD']"
              >
                <template v-slot:prepend>
                  <q-icon :name="fasCalendarDay" class="cursor-pointer">
                    <q-popup-proxy
                      transition-show="scale"
                      transition-hide="scale"
                    >
                      <q-date
                        v-model="user.dateOfStart"
                        mask="YYYY-MM-DD"
                      ></q-date>
                    </q-popup-proxy>
                  </q-icon>
                </template>
              </q-input>
            </div>
            <div class="col-12 col-sm-4 col-lg-4">
              <span class="q-mr-xs">Role</span>
              <q-radio
                v-model="user.role"
                size="xs"
                val="cashier"
                label="Cashier"
              />
              <q-radio
                v-model="user.role"
                size="xs"
                val="manager"
                label="Manager"
              />
            </div>
            <div class="col-12 col-sm-4 col-lg-4">
              <q-input
                v-model="user.employeeId"
                label="Login"
                dense
                :readonly="!isNewUser"
                :rules="[(v) => v.length > 0 || 'Required']"
              ></q-input>
            </div>
            <div class="col-12 col-sm-4 col-lg-4">
              <q-input
                v-model="user.password"
                label="Password"
                dense
                lazy-rules
                :rules="[
                  (v) =>
                    (user.employeeId !== '' && v === null) ||
                    /[0-9]+/.test(v) ||
                    'At least letters and digits',
                  (v) =>
                    (user.employeeId !== '' && v === null) ||
                    /[A-Za-z]+/.test(v) ||
                    'At least letters and digits',
                  (v) =>
                    (user.employeeId !== '' && v === null) ||
                    (v !== null && v.length > 5) ||
                    'At least 6 symbols',
                ]"
              ></q-input>
            </div>
          </div>
        </q-card-section>
        <q-card-section>
          <div class="col-12 col-sm-4 col-lg-4 text-right">
            <q-btn
              v-if="
                user.surname.length > 0 &&
                user.name.length > 0 &&
                user.city.length > 0 &&
                user.street.length > 0 &&
                user.phoneNumber.length > 0 &&
                user.salary > 0 &&
                user.street.length > 0 &&
                user.zipCode.length > 0 &&
                dateRule(user.dateOfBirth) &&
                dateRule(user.dateOfStart) &&
                ((user.employeeId !== '' && user.password === null) ||
                  (/[0-9]+/.test(user.password) &&
                    /[A-Za-z]+/.test(user.password) &&
                    user.password.length > 5))
              "
              @click="saveUser"
              flat
              round
              :icon="fasCheck"
              color="positive"
            ></q-btn>
            <q-btn @click="cancelEdit" flat round :icon="fasXmark"></q-btn>
            <q-btn
              v-if="!isNewUser"
              @click="deleteUser"
              flat
              round
              :icon="fasTrash"
              color="negative"
            ></q-btn>
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script setup>
import { ref } from "vue";
import { api } from "boot/axios";
import { useQuasar } from "quasar";
import notify from "../notices";
import {
  fasUserGroup,
  fasPlus,
  fasXmark,
  fasCheck,
  fasTrash,
  fasFilter,
  fasCalendarDay,
} from "@quasar/extras/fontawesome-v6";

defineOptions({
  name: "EmployeesPage",
});

const url = "employee";
const $q = useQuasar();
const loading = ref(true);
const dlg = ref(false);
const users = ref([]);
const user = ref(null);
const isNewUser = ref(false);
const roles = [
  { value: "cashier", label: "Cachier" },
  { value: "manager", label: "Manager" },
];
const roleLabel = (role) => roles.find((item) => item.value === role)?.label;
const animateControls = ref(false);
const animateOnEdit = () => {
  if (user.value === null) return;
  animateControls.value = true;
  window.setTimeout(() => {
    animateControls.value = false;
  }, 350);
};
const filter = ref({ role: null, filter: null });
const pagination = {
  page: 1,
  rowsPerPage: 10,
  sortBy: "fullName",
  descending: false,
};

const columns = [
  {
    name: "fullName",
    label: "Full name",
    field: "fullName",
    align: "left",
    sortable: true,
  },
  {
    name: "role",
    label: "Role",
    field: "role",
    align: "left",
    sortable: false,
  },
  {
    name: "dateOfBirth",
    label: "Birth Date",
    field: "dateOfBirth",
    align: "left",
    sortable: true,
  },
  {
    name: "phoneNumber",
    label: "Phone Number",
    field: "phoneNumber",
    align: "left",
    sortable: false,
  },
  {
    name: "employeeId",
    label: "Login",
    field: "employeeId",
    align: "left",
    sortable: false,
  },
];

const dateRule = (v) => /^\d{4}-\d{2}-\d{2}$/.test(v);

const startEdit = (id) => {
  if (user.value !== null) return;
  isNewUser.value = false;
  getUser(id);
  dlg.value = true;
};

const saveUser = () => {
  if (user.value === null) return;
  const req = isNewUser.value
    ? api.post(url, user.value)
    : api.put(url, user.value);
  req
    .then((response) => {
      if (response.data === true) {
        let shortUser = null;
        if (isNewUser.value) {
          shortUser = {
            fullName: `${user.value.surname} ${user.value.name} ${user.value.patronymic}`,
            role: user.value.role,
            dateOfBirth: user.value.dateOfBirth,
            phoneNumber: user.value.phoneNumber,
            employeeId: user.value.employeeId,
          };
          users.value.push(shortUser);
        } else {
          const index = users.value.findIndex(
            (el) => el.employeeId === user.value.employeeId
          );
          if (index > -1) {
            shortUser = users.value[index];
            shortUser.fullName = `${user.value.surname} ${user.value.name} ${user.value.patronymic}`;
            shortUser.role = user.value.role;
            shortUser.dateOfBirth = user.value.dateOfBirth;
            shortUser.phoneNumber = user.value.phoneNumber;
            shortUser.employeeId = user.value.employeeId;
          }
        }
        user.value = null;
        dlg.value = false;
        notify.success("Save successed");
      } else notify.error("Cannot save employee");
    })
    .catch((error) => {
      if (typeof error.response?.data === "string") {
        console.error(`SaveUser: ${error.message}: ${error.response?.data}`);
        notify.error(`${error.message}: ${error.response?.data}`);
      } else {
        const msg =
          `\n${error.response?.data.title}\n` +
          Object.entries(error.response?.data?.errors).reduce(
            (s, [key, value]) => s + `${key}: ${value}\n`,
            ""
          );
        console.error(`SaveUser: ${error.message}: ${msg}`);
        notify.error(`${error.message}: ${msg}`);
      }
    });
};

const filterMethod = (rows, terms) => {
  return rows.filter(
    (el) =>
      (terms.filter === null || el.fullName.includes(terms.filter)) &&
      (terms.role === null || el.role === terms.role)
  );
};

const removeUser = (id) => {
  const index = users.value.findIndex((el) => el.employeeId === id);
  if (index > -1) {
    users.value.splice(index, 1);
  }
};

const cancelEdit = () => {
  user.value = null;
  dlg.value = false;
};

const deleteUser = () => {
  if (user.value === null) return;
  notify.confirm("Record will be deleted", "Are you sure?").onOk(() => {
    api
      .delete(`${url}/${user.value.employeeId}`)
      .then((response) => {
        if (response.data) {
          removeUser(user.value.employeeId);
          user.value = null;
          dlg.value = false;
          notify.success("Delete success");
        } else {
          notify.error("Delete error");
        }
      })
      .catch((error) => {
        notify.error(`Error: ${error.message}`);
      });
  });
};

const newUser = () => {
  if (user.value !== null) return;
  isNewUser.value = true;
  user.value = {
    city: "",
    dateOfBirth: "",
    dateOfStart: "",
    employeeId: "",
    name: "",
    password: "",
    patronymic: "",
    phoneNumber: "",
    role: "cashier",
    salary: 0,
    street: "",
    surname: "",
    zipCode: "",
  };
  dlg.value = true;
};

const getUsers = () => {
  loading.value = true;
  api
    .get(url)
    .then((response) => {
      users.value = response.data;
    })
    .catch((error) => {
      notify.error(`Error: ${error.message}`);
    })
    .finally(() => (loading.value = false));
};

const getUser = (id) => {
  api
    .get(`${url}/${id}`)
    .then((response) => {
      user.value = response.data;
    })
    .catch((error) => {
      notify.error(`Error: ${error.message}`);
    });
};

getUsers();
</script>
