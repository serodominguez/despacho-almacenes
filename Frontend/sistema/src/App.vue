<template>
  <v-app id="app">
    <v-navigation-drawer
      fixed
      :clipped="$vuetify.breakpoint.mdAndUp"
      app
      v-model="drawer"
      v-if="logueado"
      temporary
    >
      <v-list dense>
        <template v-if="esAdministracion">
          <v-list-tile :to="{ name: 'home' }">
            <v-list-tile-action>
              <v-icon>home</v-icon>
            </v-list-tile-action>
            <v-list-tile-title> Inicio </v-list-tile-title>
          </v-list-tile>
        </template>
        <template v-if="esAdministracion">
          <v-list-group>
            <v-list-tile slot="activator">
              <v-list-tile-content>
                <v-list-tile-title> Almacén </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
            <v-list-tile :to="{ name: 'salidas' }">
              <v-list-tile-action>
                <v-icon>store</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title> Salidas </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
          </v-list-group>
        </template>
      </v-list>
    </v-navigation-drawer>
    <v-toolbar
      color="red darken-1"
      dark
      app
      :clipped-left="$vuetify.breakpoint.mdAndUp"
      fixed
    >
      <v-toolbar-title style="width: 300px" class="ml-0 pl-3">
        <v-toolbar-side-icon
          v-if="logueado"
          @click.stop="drawer = !drawer"
        ></v-toolbar-side-icon>
        <span v-if="logueado" class="hidden-sm-and-down">Almacén</span>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <span v-if="logueado"
        ><strong>Usuario: {{ this.$store.state.usuario.usuario }}</strong></span
      >
      <v-btn @click="salir" v-if="logueado" icon>
        <v-icon>logout</v-icon>
      </v-btn>
    </v-toolbar>
    <v-content>
      <v-container fluid fill-height>
        <v-slide-y-transition mode="out-in">
          <router-view />
        </v-slide-y-transition>
      </v-container>
    </v-content>
    <v-footer dark height="auto">
      <v-layout justify-center>
        <v-flex text-xs-center>
          <v-card flat tile color="primary" class="white--text">
            <v-card-text class="white--text pt-0"
              ><strong
                >Versión 1.0.14 - {{ new Date().getFullYear() }}</strong
              ></v-card-text
            >
          </v-card>
        </v-flex>
      </v-layout>
    </v-footer>
  </v-app>
</template>

<script>
history.pushState(null, null, location.href);
window.onpopstate = function () {
history.go(1);
};
export default {
  name: "App",
  data() {
    return {
      clipped: false,
      drawer: true,
      fixed: false,
      items: [
        {
          icon: "bubble_chart",
          title: "Inspire",
        },
      ],
      miniVariant: false,
      right: true,
      rightDrawer: false,
      title: "Vuetify.js",
    };
  },
  computed: {
    logueado() {
      return this.$store.state.usuario;
    },
    esAdministracion() {
      return (
        this.$store.state.usuario &&
        this.$store.state.usuario.rol == "administrador"
      );
    },
  },
  created() {
    this.$store.dispatch("autoLogin");
  },
  methods: {
    salir() {
      this.$store.dispatch("salir");
    },
  },
};
</script>
