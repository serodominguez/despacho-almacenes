import Vue from 'vue'
import {
  Vuetify,
  VApp,
  VNavigationDrawer,
  VFooter,
  VList,
  VBtn,
  VIcon,
  VGrid,
  VToolbar,
  VCheckbox,
  VAutocomplete,
  VCard,
  VDivider,
  VDialog,
  VTextField,
  VDataTable,
  VSelect,
  transitions
} from 'vuetify'
import 'vuetify/src/stylus/app.styl'

Vue.use(Vuetify, {
  components: {
    VApp,
    VNavigationDrawer,
    VFooter,
    VList,
    VBtn,
    VIcon,
    VGrid,
    VToolbar,
    VCheckbox,
    VAutocomplete,
    VCard,
    VDivider,
    VDialog,
    VTextField,
    VDataTable,
    VSelect,
    transitions
  },
  theme: {
    primary: '#FF1C2B',
    secondary: '#BF1520',
    accent: '#E61927',
    error: '#000000',
    info: '#BF1520',
    success: '#800E16',
    warning: '#FFC107'
  },
})
