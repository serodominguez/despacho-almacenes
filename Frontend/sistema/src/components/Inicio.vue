<template>
  <v-layout align-center justify-center>
    <v-flex xs12 sm8 md6 lg5 xl4>
      <v-card>
        <v-toolbar dark color="red darken-1">
          <v-toolbar-title>Inicio de sesión</v-toolbar-title>
        </v-toolbar>
        <v-card-text>
          <v-select
            v-model="pk_entidad"
            :items="items"
            v-on:change="seleccionarU()"
            label="Almacén"
          ></v-select>
          <v-autocomplete
            v-model="login"
            :items="usuarios"
            label="Usuario"
            no-data-text="No hay datos disponibles"
          >
          </v-autocomplete>
          <v-flex class="red--text font-weight-bold" v-if="error"
            ><strong>{{ error }}</strong></v-flex
          >
        </v-card-text>
        <v-card-actions class="px-3 pb-3">
          <v-flex text-xs-right>
            <v-btn @click="ingresar" color="primary">Ingresar</v-btn>
          </v-flex>
        </v-card-actions>
      </v-card>
    </v-flex>
  </v-layout>
</template>
<script>
import axios from "axios";
export default {
  data() {
    return {
      items: ["510","511", "520", "521", "530", "531", "509", "631"],
      usuarios: [],
      login: "",
      pk_entidad: "",
      error: null,
    };
  },
  methods: {
    uppercase() {
      this.login = this.login.toUpperCase();
    },
    ingresar() {
      this.error = null;
      axios
        .post("api/Salidas/Inicio", {
          login: this.login,
          pk_entidad: this.pk_entidad,
        })
        .then((respuesta) => {
          return respuesta.data;
        })
        .then((data) => {
          this.$store.dispatch("guardarToken", data.token);
          this.$router.push({ name: "home" });
        })
        .catch((err) => {
          if (err.response.status == 400) {
            this.error = "No es un usuario válido.";
          } else if (err.response.status == 404) {
            this.error = "No existe el usuario o sus datos son incorrectos.";
          } else if (err.response.status == 500) {
            this.error = "Servidor no disponible.";
          } else {
            this.error = "Ocurrio un error.";
          }
        });
    },
    seleccionarU() {
      let me = this;
      me.usuarios = [];
      var usuarioArray = [];
      axios
        .get("api/Salidas/CargarUsuario/" + this.pk_entidad)
        .then(function (response) {
          usuarioArray = response.data;
          usuarioArray.map(function (x) {
          me.usuarios.push({ text: x.login, value: x.pk_usuario });
          });
        })
        .catch(function (error) {
          if (error.response.status == 401) {
            me.$store.dispatch("salir");
          } else {
            console.log(error);
          }
        });
    },
  },
};
</script>
<style scoped>
.code input {
  text-transform: uppercase;
}
</style>