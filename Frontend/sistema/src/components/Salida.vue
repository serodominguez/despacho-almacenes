<template>
  <v-layout align-start>
    <v-flex>
      <v-toolbar flat color="white">
        <v-toolbar-title>Salidas {{ this.$store.state.usuario.entidad }}</v-toolbar-title>
        <v-divider class="mx-2" inset vertical></v-divider>
        <v-spacer></v-spacer>
        <span><strong>Semana: {{ semana.pK_SEMANA }} </strong></span>
        <v-btn v-if="verNuevo == 0" @click="mostrarNuevo" color="primary" dark class="mb-2">Nuevo</v-btn>
        <v-dialog v-model="wait" persistent max-width="500">
          <v-card color="primary" dark>
            <v-card-text>
              Espere por favor......
            </v-card-text>
          </v-card>
        </v-dialog>
        <v-dialog v-model="verOrdenes" persistent max-width="1000px">
          <v-card>
            <v-card-title>
              <span class="headline">Seleccione las ordenes de pedidos.</span>
            </v-card-title>
            <v-card-text>
              <v-container grid-list-md>
                <v-layout wrap>
                  <v-flex xs12 sm12 md12 lg12 xl12>
                    <template>
                      <v-data-table v-model="selected" :headers="cabeceraOrdenes" :items="ordenes" select-all
                        item-key="pK_ORDEN_PED" :rows-per-page-text="paginas" class="elevation-1">
                        <template slot-scope="props">
                          <tr>
                            <th>
                              <v-checkbox :input-value="props.all" :indeterminate="props.indeterminate" primary
                                hide-details @click.stop="toggleAll"></v-checkbox>
                            </th>
                          </tr>
                        </template>
                        <template slot="items" slot-scope="props">
                          <tr :active="props.selected" @click="props.selected = !props.selected">
                            <td>
                              <v-checkbox :input-value="props.selected" primary hide-details></v-checkbox>
                            </td>
                            <td>{{ props.item.nrO_ORDEN }}</td>
                            <td>{{ props.item.pK_ORDEN_PED }}</td>
                            <td>{{ props.item.pK_SEMANA }}</td>
                            <td>{{ props.item.obs }}</td>
                          </tr>
                        </template>
                        <template slot="no-data">
                          <h3>No hay artículos para mostrar.</h3>
                        </template>
                      </v-data-table>
                    </template>
                  </v-flex>
                </v-layout>
              </v-container>
            </v-card-text>
            <v-card-actions>
              <div class="red--text font-weight-bold" v-for="v in marcaMensaje" :key="v" v-text="v"></div>
              <v-spacer></v-spacer>
              <v-btn @click="guardarOrdenes()" color="primary">Aceptar</v-btn>
              <v-btn @click="ocultarOrdenes()" color="error">Cancelar</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-toolbar>
      <v-data-table :headers="headers" :items="salidas" class="elevation-1" v-if="verNuevo == 0"
        :rows-per-page-text="pagina">
        <template slot="items" slot-scope="props">
          <td>{{ props.item.nrO_DOC }}</td>
          <td>{{ props.item.tipO_SALIDA }}</td>
          <td>{{ props.item.pK_ENTIDAD }}</td>
          <td>{{ props.item.pK_SEMANA }}</td>
          <td>{{ props.item.fechA_DOC }}</td>
          <td>{{ props.item.nombre }}</td>
          <td>{{ props.item.totaL_PREPACKS }}</td>
          <td>{{ props.item.totaL_PARES }}</td>
          <td>{{ props.item.estado }}</td>
          <td>{{ props.item.login }}</td>
        </template>
        <template slot="no-data">
          <v-btn color="primary" @click="listar">Resetear</v-btn>
        </template>
      </v-data-table>
      <v-container grid-list-sm class="pa-4 white" v-if="verNuevo">
        <v-layout row wrap>
          <v-flex xs12 sm8 md8 lg3 xl3>
            <v-text-field v-model="tipo" label="Tipo de Despacho" readonly>
            </v-text-field>
          </v-flex>
          <v-flex xs12 sm4 md4 lg2 xl2>
            <v-select v-model="ciudad" :items="ciudades" label="Ciudad" v-on:change="seleccionarS()"
              no-data-text="No hay datos disponibles"></v-select>
          </v-flex>
          <v-flex xs4 sm4 md4 lg1 xl1>
            <v-select v-model="pk_tienda" :items="tiendas" label="Código" v-on:change="encuentraTienda(pk_tienda)"
              no-data-text="No hay datos disponibles">
            </v-select>
          </v-flex>
          <v-flex xs6 sm6 md6 lg4 xl4>
            <v-text-field v-model="nombre" label="Tienda" readonly>
            </v-text-field>
          </v-flex>
          <v-flex xs2 sm2 md2 lg2 xl2>
            <div v-if="pk_tienda == ''">
              <v-btn small fab dark color="primary">
                <v-icon dark>list_alt</v-icon>
              </v-btn>
            </div>
            <div v-else>
              <v-btn @click="mostrarOrdenes()" small fab dark color="primary">
                <v-icon dark>list_alt</v-icon>
              </v-btn>
            </div>
          </v-flex>
          <v-flex xs6 sm6 md6 lg2 xl2>
            <v-text-field v-model="concepto" label="Concepto" readonly>
            </v-text-field>
          </v-flex>
          <v-flex xs6 sm6 md6 lg2 xl2>
            <v-text-field v-model="fecha" label="Fecha" readonly>
            </v-text-field>
          </v-flex>
          <v-flex xs12 sm8 md8 lg6 xl6>
            <v-text-field maxlength="100" v-model="observaciones" label="Observaciones">
            </v-text-field>
          </v-flex>
          <v-flex xs10 sm8 md8 lg8 xl8>
            <v-text-field maxlength="13" v-model="codigo" label="Código de Barras"
              onkeypress="return (event.charCode >= 48 && event.charCode <= 57)" @keyup.enter="buscarCodigo()">
            </v-text-field>
          </v-flex>
          <v-flex xs2 sm2 md2 lg2 xl2>
            <div>
              <v-btn @click="buscarCodigo()" small fab dark color="primary">
                <v-icon dark>login</v-icon>
              </v-btn>
            </div>
          </v-flex>
          <v-flex xs12 sm2 md2 lg2 xl2 v-if="errorArticulo">
            <div class="red--text font-weight-bold" style="font-size: 20px" v-text="errorArticulo"></div>
          </v-flex>
          <v-flex xs12 sm12 md10 lg10 xl10>
            <v-data-table :headers="cabeceraDetalles" :items="detalles" hide-actions class="elevation-1">
              <template slot="items" slot-scope="props">
                <td class="justify-center layout px-0">
                  <v-icon small class="mr-2" @click="eliminarDetalle(detalles, props.item)">
                    delete
                  </v-icon>
                </td>
                <td>{{ props.item.pK_ARTICULO }}</td>
                <td>{{ props.item.pK_PPREPACK }}</td>
                <td>{{ props.item.cpares }}</td>
                <td>{{ props.item.cprepacks }}</td>
                <td>{{ props.item.pventa }}</td>
                <td>{{ props.item.taM1 }}</td>
                <td>{{ props.item.taM2 }}</td>
                <td>{{ props.item.taM3 }}</td>
                <td>{{ props.item.taM4 }}</td>
                <td>{{ props.item.taM5 }}</td>
                <td>{{ props.item.taM6 }}</td>
                <td>{{ props.item.taM7 }}</td>
                <td>{{ props.item.taM8 }}</td>
                <td>{{ props.item.taM9 }}</td>
              </template>
              <template slot="no-data">
                <h3>No hay artículos agregados al detalle.</h3>
              </template>
            </v-data-table>
            <v-flex class="text-xs-right">
              <strong>Total Cajas: </strong>
              {{ (totalprepacks = calcularTotalPrePacks) }}
            </v-flex>
            <v-flex class="text-xs-right">
              <strong>Total Pares: </strong>
              {{ (totalpares = calcularTotalPares) }}
            </v-flex>
          </v-flex>
          <v-flex xs6 sm6 md5 lg5 xl5>
            <v-autocomplete v-model="pk_despachador" :items="despachadores" label="Despachador"
              no-data-text="No hay datos disponibles">
            </v-autocomplete>
          </v-flex>
          <v-flex xs6 sm6 md5 lg5 xl5>
            <v-autocomplete v-model="pk_revisor" :items="revisores" label="Revisor"
              no-data-text="No hay datos disponibles">
            </v-autocomplete>
          </v-flex>
          <v-flex xs6 sm6 md5 lg5 xl5>
            <v-autocomplete v-model="pk_transportador" :items="transportistas" label="Transportista"
              v-on:change="seleccionarV()" no-data-text="No hay datos disponibles">
            </v-autocomplete>
          </v-flex>
          <v-flex xs6 sm6 md5 lg5 xl5>
            <v-select v-model="placa" :items="vehiculos" label="Placa" no-data-text="No hay datos disponibles">
            </v-select>
          </v-flex>
          <v-flex xs12 sm12 md12 lg12 xl12>
            <div class="red--text font-weight-bold" v-for="v in validaMensaje" :key="v" v-text="v"></div>
          </v-flex>
          <v-flex xs12 sm2 md2 lg2 xl2 v-if="errorPedido">
            <div class="red--text font-weight-bold" style="font-size: 20px" v-text="errorPedido"></div>
          </v-flex>
          <v-flex xs12 sm2 md2 lg2 xl2>
              <li class="red--text font-weight-bold" v-for="item in items" v-bind:key="item.pK_ARTICULO">
              {{ item.pK_ARTICULO }}
            </li>
          </v-flex>
          <v-flex xs12 sm12 md12 lg12 xl12>
            <v-btn :disabled="wait" :loading="wait" v-if="verDetalle == 0" @click="guardar()" color="primary">Guardar</v-btn>
            <v-btn @click="validarOrdenes()" color="green darken-4" dark class="mb-2">Validar</v-btn>
            <v-btn @click="ocultarNuevo()" color="error">Cancelar</v-btn>
          </v-flex>
        </v-layout>
      </v-container>
    </v-flex>
  </v-layout>
</template>
<script>
import axios from "axios";
export default {
  data() {
    return {
      enableDisable: false,
      detecteds: [],
      selected: [],
      items: [],
      salidas: [],
      wait: false,
      dialog: false,
      headers: [
        {
          text: "Nº Documento",
          value: "nro_DOC",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Tipo de Salida",
          value: "tipO_SALIDA",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Sección",
          value: "pK_ENTIDAD",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Semana",
          value: "pK_SEMANA",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Fecha Documento",
          value: "fechA_DOC",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Tienda de Destino",
          value: "nombre",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Cajas",
          value: "totaL_PREPACKS",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Pares",
          value: "totaL_PARES",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Estado",
          value: "estado",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Usuario",
          value: "login",
          sortable: false,
          class: "black--text font-weight-bold",
        },
      ],
      cabeceraDetalles: [
        {
          text: "Borrar",
          value: "borrar",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Artículo",
          value: "pK_ARTICULO",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "PrePack",
          value: "pK_PPREPACK",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Pares",
          value: "cpares",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Cajas",
          value: "cprepacks",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Precio",
          value: "pventa",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "1",
          value: "taM1",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "2",
          value: "taM2",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "3",
          value: "taM3",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "4",
          value: "taM4",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "5",
          value: "taM5",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "6",
          value: "taM6",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "7",
          value: "taM7",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "8",
          value: "taM8",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "9",
          value: "taM9",
          sortable: false,
          class: "black--text font-weight-bold",
        },
      ],
      cabeceraOrdenes: [
        {
          text: "Nº Orden",
          value: "nrO_ORDEN",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Orden de Pedido",
          value: "pK_ORDEN_PED",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Semana",
          value: "pK_SEMANA",
          sortable: false,
          class: "black--text font-weight-bold",
        },
        {
          text: "Observaciones",
          value: "obs",
          sortable: false,
          class: "black--text font-weight-bold",
        },
      ],
      detalles: [],
      ordenes: [],
      ppack: "",
      id: "",
      t1: "",
      t2: "",
      t3: "",
      t4: "",
      t5: "",
      t6: "",
      t7: "",
      t8: "",
      t9: "",
      pack: "",
      pk_tienda: "",
      pk_tipo_doc: 3,
      pk_tipo_mov: 0,
      pk_calidad: 1,
      pk_despachador: "",
      pk_revisor: "",
      pk_transportador: "",
      placa: "",
      nombre: "",
      empleado: "",
      ciudad: "",
      totalpares: 0,
      totalprepacks: 0,
      semana: [],
      tipo_tra: "ND",
      concepto: "IMP",
      ess_prepack: "S",
      tipo: "DESPACHO A TIENDAS",
      //fecha: new Date().toLocaleDateString(),
      fecha: "",
      tiendas: [],
      pedidos: [],
      despachadores: [],
      revisores: [],
      transportistas: [],
      vehiculos: [],
      ciudades: [
        "Beni",
        "Cochabamba",
        "La Paz",
        "Oruro",
        "Pando",
        "Potosi",
        "Santa Cruz",
        "Sucre",
        "Tarija",
      ],
      numero: "",
      observaciones: "",
      codebar: "",
      codigo: "",
      estado: "V",
      parametro: "",
      pagina: "Salidas por página",
      paginas: "Ordenes por página",
      verNuevo: 0,
      errorArticulo: null,
      errorPedido: null,
      verOrdenes: 0,
      valida: 0,
      validaMensaje: [],
      marca: 0,
      marcaMensaje: [],
      adModal: 0,
      adAccion: 0,
      adNombre: "",
      adId: "",
      verDetalle: 0,
      totalA: 0,
      totalB: 0,
    };
  },
  computed: {
    calcularTotalPares: function () {
      var resultado = 0;
      for (var i = 0; i < this.detalles.length; i++) {
        resultado = resultado + this.detalles[i].cpares;
      }
      return resultado;
    },
    calcularTotalPrePacks: function () {
      var resultado = 0;
      for (var i = 0; i < this.detalles.length; i++) {
        resultado = resultado + this.detalles[i].cprepacks;
      }
      return resultado;
    },
  },
  watch: {
    dialog(val) {
      val || this.close();
    },
  },
  created() {
    let d = new Date();
    let ye = new Intl.DateTimeFormat('en', { year: 'numeric' }).format(d);
    let mo = new Intl.DateTimeFormat('en', { month: 'short' }).format(d);
    let da = new Intl.DateTimeFormat('en', { day: 'numeric' }).format(d);
    this.fecha = (`${da}-${mo}-${ye}`);
    this.listar();
    this.obtenerSemana(this.$store.state.usuario.entidad);
    this.seleccionarD();
    this.seleccionarR();
    this.seleccionarT();
  },
  methods: {
    toggleAll() {
      if (this.selected.length) this.selected = [];
      else this.selected = this.desserts.slice();
    },
    mostrarNuevo() {
      this.verNuevo = 1;
    },
    ocultarNuevo() {
      this.verNuevo = 0;
      this.limpiar();
    },
    mostrarOrdenes() {
      let me = this;
      me.verOrdenes = 1;
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      axios
        .get("api/Salidas/CargarOrdenes/" + me.pk_tienda + "/" + me.$store.state.usuario.entidad, configuracion)
        .then(function (response) {
          me.ordenes = response.data;
        })
        .catch(function (error) {
          if (error.response.status == 401) {
            me.$store.dispatch("salir");
          } else {
            console.log(error);
          }
        });
    },
    guardarOrdenes() {
      if (this.marcar()) {
        return;
      }
      var resultarray = [];
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      let me = this;
      axios
        .post(
          "api/Salidas/LeerPedidos",
          {
            pK_TIENDA: me.pk_tienda,
            ordenes: me.selected,
          },
          configuracion
        )
        .then(function (response) {
          resultarray = response.data;
          const result = resultarray.reduce((acumulador, valorActual) => {
            const elementoYaExiste = acumulador.find(
              (elemento) =>
                elemento.pK_ARTICULO === valorActual.pK_ARTICULO &&
                elemento.taM1 === valorActual.taM1 &&
                elemento.taM12 === valorActual.taM2 &&
                elemento.taM3 === valorActual.taM3 &&
                elemento.taM4 === valorActual.taM4 &&
                elemento.taM5 === valorActual.taM5 &&
                elemento.taM6 === valorActual.taM6 &&
                elemento.taM7 === valorActual.taM7 &&
                elemento.taM8 === valorActual.taM8 &&
                elemento.taM9 === valorActual.taM9
            );
            if (elementoYaExiste) {
              return acumulador.map((elemento) => {
                if (
                  elemento.pK_ARTICULO === valorActual.pK_ARTICULO &&
                  elemento.taM1 === valorActual.taM1 &&
                  elemento.taM2 === valorActual.taM2 &&
                  elemento.taM3 === valorActual.taM3 &&
                  elemento.taM4 === valorActual.taM4 &&
                  elemento.taM5 === valorActual.taM5 &&
                  elemento.taM6 === valorActual.taM6 &&
                  elemento.taM7 === valorActual.taM7 &&
                  elemento.taM8 === valorActual.taM8 &&
                  elemento.taM9 === valorActual.taM9
                ) {
                  return {
                    ...elemento,
                    totaL_PARES: elemento.totaL_PARES + valorActual.totaL_PARES,
                  };
                }
                return elemento;
              });
            }
            return [...acumulador, valorActual];
          }, []);
          me.pedidos = result;
        })
        .catch(function (error) {
          if (error.response.status == 401) {
            me.$store.dispatch("salir");
          } else {
            console.log(error);
          }
        });
      this.verOrdenes = 0;
      this.marca = 0;
      this.marcaMensaje = [];
    },
    ocultarOrdenes() {
      this.verOrdenes = 0;
      this.ordenes = [];
      this.selected = [];
      this.marca = 0;
      this.marcaMensaje = [];
    },
    buscarCodigo() {
      let me = this;
      me.errorArticulo = null;
      if (!isNaN(this.codigo)) {
        let header = { Authorization: "Bearer " + this.$store.state.token };
        let configuracion = { headers: header };
        axios
          .get(
            "api/Salidas/BuscarArticulo/" + this.codigo.match(/.{1,8}/g) + "/" + this.semana.pK_SEMANA + "/" + this.$store.state.usuario.entidad + "/" + this.selected[0].pK_ORDEN_PED + "/" + this.pk_tienda,
            configuracion
          )
          .then(function (response) {
            me.agregarDetalle(response.data);
          })
          .catch(function (error) {
            switch (error.response.status) {
              case 400:
                me.errorArticulo = "No tiene un plan asignado";
                me.codigo = "";
                break;
              case 404:
                me.errorArticulo = "No existe el artículo";
                //me.codigo = "";
                break;
            }
          });
      }
    },
    agregarDetalle(data = []) {
      this.errorArticulo = null;
    if (!this.encuentraArticulo(data["pK_ARTICULO"],Number(data["pK_PPREPACK"]))) {
        this.errorArticulo = "El artículo no se encuentra en los pedidos";
        this.codigo = "";
      } else {
        if (
          this.recorrerListas(
            data["pK_ARTICULO"],
            data["pK_PPREPACK"],
            data["taM1"],
            data["taM2"],
            data["taM3"],
            data["taM4"],
            data["taM5"],
            data["taM6"],
            data["taM7"],
            data["taM8"],
            data["taM9"]
          )
        ) {
          this.errorArticulo = "La cantidad de pares esta excedida";
          this.codigo = "";
        } else {
          if (!this.encuentra(data["pK_ARTICULO"],Number(data["pK_PPREPACK"]),data["cpares"])) {
              this.detalles.push({
              pK_ARTICULO: data["pK_ARTICULO"],
              pK_PPREPACK: data["pK_PPREPACK"],
              pK_MARCA: data["pK_MARCA"],
              pK_CATEGORIA: data["pK_CATEGORIA"],
              pK_SUBCATEGORIA: data["pK_SUBCATEGORIA"],
              pK_ENTIDAD: data["pK_ENTIDAD"],
              pK_PLAN: data["pK_PLAN"],
              pK_CANAL: data["pK_CANAL"],
              cb: data["cb"],
              taM1: data["taM1"],
              taM2: data["taM2"],
              taM3: data["taM3"],
              taM4: data["taM4"],
              taM5: data["taM5"],
              taM6: data["taM6"],
              taM7: data["taM7"],
              taM8: data["taM8"],
              taM9: data["taM9"],
              cprepacks: 1,
              pventa: data["pventa"],
              pcosto: data["pcosto"],
              cpares: data["cpares"],
              pK_CALIDAD: this.pk_calidad,
            });
            this.codigo = "";
          }
        }
      }
    },
    recorrerListas(id, pack, t1, t2, t3, t4, t5, t6, t7, t8, t9) {
      var sw = 0;
      for (var j = 0; j < this.detalles.length; j++) {
        if (
          this.detalles[j].pK_ARTICULO == id &&
          this.detalles[j].pK_PPREPACK == pack
        ) {
          for (var i = 0; i < this.pedidos.length; i++) {
            if (
              this.pedidos[i].pK_ARTICULO == id &&
              this.pedidos[i].taM1 == t1 &&
              this.pedidos[i].taM2 == t2 &&
              this.pedidos[i].taM3 == t3 &&
              this.pedidos[i].taM4 == t4 &&
              this.pedidos[i].taM5 == t5 &&
              this.pedidos[i].taM6 == t6 &&
              this.pedidos[i].taM7 == t7 &&
              this.pedidos[i].taM8 == t8 &&
              this.pedidos[i].taM9 == t9
            ) {
              parseInt(this.detalles[j].cpares) <=
                parseInt(this.pedidos[i].totaL_PARES);
              sw = 1;
            }
          }
        }
      }
      return sw;
    },
    encuentra(id, pack, tpares) {
      var sw = 0;
      for (var i = 0; i < this.detalles.length; i++) {
        if (this.detalles[i].pK_ARTICULO == id && this.detalles[i].pK_PPREPACK == pack) {
          this.detalles[i].cpares += parseInt(tpares);
          this.detalles[i].cprepacks += 1;
          sw = 1;
        }
      }
      this.codigo = "";
      return sw;
    },
    encuentraArticulo(id, ppack) {
      var sw = 0;
      for (var i = 0; i < this.pedidos.length; i++) {
        if (this.pedidos[i].pK_ARTICULO == id && this.pedidos[i].pK_PPACK == ppack) {
          sw = 1;
        }
      }
      return sw;
    },
    encuentraTienda(id) {
      for (var i = 0; i < this.tiendas.length; i++) {
        if (this.tiendas[i].value == id) {
          this.nombre = this.tiendas[i].text2;
        }
      }
    },
    eliminarDetalle(arr, item) {
      var i = arr.indexOf(item);
      if (i !== -1) {
        arr.splice(i, 1);
      }
    },
    obtenerSemana(pk) {
      let me = this;
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      axios
        .get("api/Salidas/ObtenerSemana/" + pk,  configuracion)
        .then(function (response) {
          me.semana = response.data;
        })
        .catch(function (error) {
          if (error.response.status == 401) {
            me.$store.dispatch("salir");
          } else {
            console.log(error);
          }
        });
    },
    seleccionarS() {
      this.parametro = this.ciudad + "," + this.$store.state.usuario.entidad;
      let me = this;
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      me.tiendas = [];
      me.nombre = "";
      var tiendaArray = [];
      axios
        .get("api/Salidas/CargarTienda/" + this.parametro, configuracion)
        .then(function (response) {
          tiendaArray = response.data;
          tiendaArray.map(function (x) {
            me.tiendas.push({
              text: x.pK_TIENDA,
              text2: x.nombre,
              value: x.pK_TIENDA,
            });
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
    seleccionarD() {
      let me = this;
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      var despachadorArray = [];
      axios
        .get("api/Salidas/CargarDespachador", configuracion)
        .then(function (response) {
          despachadorArray = response.data;
          despachadorArray.map(function (x) {
            me.despachadores.push({ text: x.empleado, value: x.nrO_DOC });
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
    seleccionarR() {
      let me = this;
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      var revisorArray = [];
      axios
        .get("api/Salidas/CargarRevisor", configuracion)
        .then(function (response) {
          revisorArray = response.data;
          revisorArray.map(function (x) {
            me.revisores.push({ text: x.empleado, value: x.nrO_DOC });
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
    seleccionarT() {
      let me = this;
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      var transportadorArray = [];
      axios
        .get("api/Salidas/CargarTransportador", configuracion)
        .then(function (response) {
          transportadorArray = response.data;
          transportadorArray.map(function (x) {
            me.transportistas.push({ text: x.empleado, value: x.nrO_DOC });
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
    seleccionarV() {
      let me = this;
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      me.vehiculos = [];
      var vehiculoArray = [];
      axios
        .get(
          "api/Salidas/CargarVehiculo/" + this.pk_transportador,
          configuracion
        )
        .then(function (response) {
          vehiculoArray = response.data;
          vehiculoArray.map(function (x) {
          me.vehiculos.push({ text: x.placa, value: x.pk_empleado });
          });
        })
        .catch(function (error) {
          if (error.response.status == 401) {
            me.$store.dispatch("salir");
          } else {
            console.log(error);
          }
        });

      for (var i = 0; i < this.transportistas.length; i++) {
        if (this.transportistas[i].value == this.pk_transportador) {
          this.empleado = this.transportistas[i].text;
        }
      }
    },
    limpiar() {
      this.id = "";
      this.t1 = "";
      this.t2 = "";
      this.t3 = "";
      this.t4 = "";
      this.t5 = "";
      this.t6 = "";
      this.t7 = "";
      this.t8 = "";
      this.t9 = "";
      this.ppack ="";
      this.pk_tienda = "";
      this.pk_despachador = "";
      this.pk_revisor = "";
      this.pk_transportador = "";
      this.placa = "";
      this.nombre = "";
      this.empleado = "";
      this.observaciones = "";
      this.detalles = [];
      this.vehiculos = [];
      this.ordenes = [];
      this.tiendas = [];
      this.selected = [];
      this.codigo = "";
      this.ciudad = "";
      this.verDetalle = 0;
      this.valida = 0;
      this.validaMensaje = [];
      this.marca = 0;
      this.marcaMensaje = [];
      this.errorArticulo = null;
      this.errorPedido = null;
      this.enableDisable = false;
      this.wait = false;
    },
    listar() {
      let me = this;
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      axios
        .get(
          "api/Salidas/Listar/" + me.$store.state.usuario.entidad,
          configuracion
        )
        .then(function (response) {
          me.salidas = response.data;
        })
        .catch(function (error) {
          if (error.response.status == 401) {
            me.$store.dispatch("salir");
          } else {
            console.log(error);
          }
        });
    },
    guardar() {
      if (this.validar()) {
        return;
      }
      this.wait = true;
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      this.enableDisable = true;
      let me = this;
      me.errorPedido = null;
      axios
        .post(
          "api/Salidas/CrearSalida",
          {
            pK_SECCION: me.$store.state.usuario.entidad,
            pK_TIPO_DOC: me.pk_tipo_doc,
            pK_TIPO_MOV: me.pk_tipo_mov,
            pK_ENTIDAD: me.pk_tienda,
            pK_SEMANA: me.semana.pK_SEMANA,
            fechA_DOC: me.fecha,
            estado: me.estado,
            pK_EMP_DES: me.pk_despachador,
            pK_EMP_REV: me.pk_revisor,
            pK_TIPO_TRA: me.tipo_tra,
            pK_EMP_TRA: me.pk_transportador,
            pK_PEDIDO: me.selected[0].pK_ORDEN_PED,
            nombrE_TRA: me.empleado,
            pK_USUARIO: me.$store.state.usuario.pk_usuario,
            placa: me.placa,
            obs: me.observaciones,
            pK_CONCEPTO: me.concepto,
            esS_PPACK: me.ess_prepack,
            totaL_PREPACKS: me.totalprepacks,
            totaL_PARES: me.totalpares,
            detalles: me.detalles,
            ordenes: me.selected,
          },
          configuracion
        )
        .then(function (response) {
          me.ocultarNuevo();
          me.listar();
          me.limpiar();
        })
        .catch(function (error) {
          if (error.response.status == 500) {
             me.errorPedido = "Error al registrar el despacho!";
             me.wait = false;
          } else {
            console.log(error);
          }
        });
    },
    validarOrdenes() {
      if (this.validar()) {
        return;
      }
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      let me = this;
      me.errorPedido = null;
      axios
        .post(
          "api/Salidas/ValidarArticulos",
          {
            pK_SECCION: me.$store.state.usuario.entidad,
            pK_TIPO_DOC: me.pk_tipo_doc,
            pK_TIPO_MOV: me.pk_tipo_mov,
            pK_ENTIDAD: me.pk_tienda,
            pK_SEMANA: me.semana.pK_SEMANA,
            fechA_DOC: me.fecha,
            estado: me.estado,
            pK_EMP_DES: me.pk_despachador,
            pK_EMP_REV: me.pk_revisor,
            pK_TIPO_TRA: me.tipo_tra,
            pK_EMP_TRA: me.pk_transportador,
            nombrE_TRA: me.empleado,
            pK_USUARIO: me.$store.state.usuario.pk_usuario,
            placa: me.placa,
            obs: me.observaciones,
            pK_CONCEPTO: me.concepto,
            esS_PPACK: me.ess_prepack,
            totaL_PREPACKS: me.totalprepacks,
            totaL_PARES: me.totalpares,
            detalles: me.detalles,
            ordenes: me.selected,
          },
          configuracion
        )
        .then(function (response) {
          me.items = response.data;
        })
        .catch(function (error) {
          if (error.response.status == 401) {
            me.$store.dispatch("salir");
          } else {
            console.log(error);
          }
        });
    },
    validarSalidas() {
      let header = { Authorization: "Bearer " + this.$store.state.token };
      let configuracion = { headers: header };
      let me = this;
      me.errorPedido = null;
      axios
        .post(
          "api/Salidas/ValidarSalidas",
          {
            pK_SECCION: me.$store.state.usuario.entidad,
            pK_TIPO_DOC: me.pk_tipo_doc,
            pK_TIPO_MOV: me.pk_tipo_mov,
            pK_ENTIDAD: me.pk_tienda,
            pK_SEMANA: me.semana.pK_SEMANA,
            fechA_DOC: me.fecha,
            estado: me.estado,
            pK_EMP_DES: me.pk_despachador,
            pK_EMP_REV: me.pk_revisor,
            pK_TIPO_TRA: me.tipo_tra,
            pK_EMP_TRA: me.pk_transportador,
            nombrE_TRA: me.empleado,
            pK_USUARIO: me.$store.state.usuario.pk_usuario,
            placa: me.placa,
            obs: me.observaciones,
            pK_CONCEPTO: me.concepto,
            esS_PPACK: me.ess_prepack,
            totaL_PREPACKS: me.totalprepacks,
            totaL_PARES: me.totalpares,
            detalles: me.detalles,
            ordenes: me.selected,
          },
          configuracion
        )
        .then(function (response) {
          me.items = response.data;
        })
        .catch(function (error) {
          if (error.response.status == 401) {
            me.$store.dispatch("salir");
          } else {
            console.log(error);
          }
        });
    },
    validar() {
      this.valida = 0;
      this.validaMensaje = [];

      if (!this.pk_tienda) {
        this.validaMensaje.push("Seleccione una tienda!");
      }
      if (this.detalles.length <= 0) {
        this.validaMensaje.push("Ingrese al menos un artículo al detalle!");
      }
      if (this.selected.length <= 0) {
        this.validaMensaje.push("Seleccione una orden de pedido!");
      }
      if (this.validaMensaje.length) {
        this.valida = 1;
      }
      return this.valida;
    },
    marcar() {
      this.marca = 0;
      this.marcaMensaje = [];

      if (this.selected.length == 0) {
        this.marcaMensaje.push("Seleccione una orden de pedido!");
      }
      if (this.marcaMensaje.length) {
        this.marca = 1;
      }
      return this.marca;
    },
  },
};
</script>
