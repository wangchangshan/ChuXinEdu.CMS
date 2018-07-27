
/**
 * 需要递归循环children,重新赋值component
 */
import lazyLoading from './lazyLoading'
const menuHelper = {
    generateRoutesFromMenu(menuData = [], routes = [], componentNew) {
        for (var i = 0; i < menuData.length; i++) {
            const menuobj = menuData[i];
            const component = menuData[i].component;

            if (component && component !== 'content') {
                //componentNew = require('@/page/' + menuData[i].component + '.vue')
                componentNew = lazyLoading(menuData[i].component)
            } else {
                //componentNew = require('@/layout/' + menuData[i].component + '.vue')
            }

            menuobj['component'] = componentNew
            routes.push(menuobj)
            this.generateRoutesFromMenu(menuobj.children)
        }
        return routes;
    }
}

export { menuHelper }

