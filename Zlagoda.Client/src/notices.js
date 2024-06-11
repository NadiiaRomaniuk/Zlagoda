import { Notify, Dialog } from "quasar";
import {
  fasXmark,
  fasCircleInfo,
  fasCircleCheck,
  fasCircleExclamation,
  fasTriangleExclamation,
} from "@quasar/extras/fontawesome-v6";

// / * eslint-disable no-use-before-define */
class Notices {
  timeout = 5000;

  info(msg, timeout = this.timeout) {
    Notify.create({
      color: "info",
      textColor: "white",
      message: msg,
      icon: String(fasCircleInfo),
      position: "top",
      timeout,
      actions: [{ icon: String(fasXmark), color: "white" }],
    });
  }

  success(msg, timeout = this.timeout) {
    Notify.create({
      color: "positive",
      textColor: "white",
      message: msg,
      icon: String(fasCircleCheck),
      position: "top",
      timeout: timeout || this.timeout,
      actions: [{ icon: String(fasXmark), color: "white" }],
    });
  }

  warning(msg, timeout = this.timeout) {
    Notify.create({
      color: "warning",
      textColor: "white",
      message: msg,
      icon: String(fasCircleExclamation),
      position: "top",
      timeout: timeout || this.timeout,
      actions: [{ icon: String(fasXmark), color: "white" }],
    });
  }

  warningList(msg) {
    Notify.create({
      color: "warning",
      textColor: "black",
      message: msg,
      icon: String(fasCircleExclamation),
      iconColor: "orange-10",
      classes: "warningList",
      position: "top",
      timeout: 0,
      html: true,
      actions: [{ icon: String(fasXmark), color: "black" }],
    });
  }

  error(msg, err, timeout = this.timeout) {
    let message;
    if (err) {
      if (err.response) {
        message = `${msg} (${err.response?.status}): ${String(
          err.response?.data
        )}`;
      } else {
        message = `${msg}: ${err.message}`;
      }
    } else {
      message = msg;
    }
    Notify.create({
      color: "negative",
      textColor: "white",
      message,
      icon: String(fasTriangleExclamation),
      position: "top",
      timeout: timeout || this.timeout,
      actions: [{ icon: String(fasXmark), color: "white" }],
    });
  }

  confirm(title, message) {
    return Dialog.create({
      title,
      class: "q-pa-sm",
      message,
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
    });
  }

  formatPrice(num, precision = 2) {
    let str;
    if (precision === 0) {
      str = "";
    } else {
      str = num.toFixed(precision);
      str = str.slice(str.indexOf("."));
      num = Math.floor(num);
    }
    return (
      num
        .toString()
        .split("")
        .reverse()
        .reduce(
          (acc, num, i) =>
            num === "-" ? acc : num + (i && !(i % 3) ? "," : "") + acc,
          ""
        ) + str
    );
  }
}

const notices = new Notices();

export default notices;

// / * eslint-enable no-use-before-define */
// app.config.globalProperties.$axios = axios;
