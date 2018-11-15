
/**
 * 需要递归循环children,重新赋值component
 */
import lazyLoading from './lazyLoading'
const menuHelper = {
    generateRoutesFromMenu(menuData = [], routes = [], componentNew) {
        for (var i = 0; i < menuData.length; i++) {
            const menuobj = menuData[i];
            const component = menuData[i].component;
            if (component && component !== 'content' && typeof component != 'function') {
                componentNew = lazyLoading.loadPageComponent(menuData[i].component)
                //componentNew = () => import('@/page/student')
            } else if(typeof component != 'function') {
                componentNew = lazyLoading.loadLayoutComponent(menuData[i].component)
            }

            menuobj['component'] = componentNew
            routes.push(menuobj)
            this.generateRoutesFromMenu(menuobj.children)
        }
        return routes;
    }
}

export { menuHelper }

