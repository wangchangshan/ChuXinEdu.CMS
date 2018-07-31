//export default (name) => () => import(`@/page/${name}.vue`)

function loadPageComponent(name){
    return () => import(`@/page/${name}.vue`)
}

function loadLayoutComponent(name){
    return () => import(`@/layout/${name}.vue`)
}

export default {
    loadPageComponent,
    loadLayoutComponent
}