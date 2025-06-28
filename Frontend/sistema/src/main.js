import 'material-design-icons-iconfont/dist/material-design-icons.css'
import '@babel/polyfill'
import Vue from 'vue'
import './plugins/vuetify'
import App from './App.vue'
import router from './router'
import store from './store'
import axios from 'axios'

Vue.config.productionTip = false
axios.defaults.baseURL='http://localhost:63038'
new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
