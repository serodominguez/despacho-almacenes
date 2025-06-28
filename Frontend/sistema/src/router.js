import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'
import Inicio from './components/Inicio.vue'
import Salida from './components/Salida.vue'
import store from './store'

Vue.use(Router)

var router = new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
      meta: {
        administrador: true
      }
    },
    {
      path: '/inicio',
      name: 'inicio',
      component: Inicio,
      meta: {
        libre: true
      }
    },
    {
      path: '/salidas',
      name: 'salidas',
      component: Salida,
      meta: {
        administrador: true
      }
    }
  ]
})

router.beforeEach((to, from, next) => {
  if(to.matched.some(record => record.meta.libre)){
    next()
  } else if (store.state.usuario && store.state.usuario.rol == 'administrador'){
    if (to.matched.some(record => record.meta.administrador)){
        next()
    }
  } else {
    next({
      name: 'inicio'
    })
  }
})
export default router