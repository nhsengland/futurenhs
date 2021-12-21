declare interface Config {
    router: any;
    paramName: string
}

export const getRouteToParam = ({
    router,
    paramName
}: Config): string => {

    try {

        const currentPathName: string = router?.asPath;
        const route: string = currentPathName?.slice(0, router.pathname?.indexOf(`[${paramName}]`)) + router?.query[paramName];
    
        return route;

    } catch(error){

        return '';

    }

};
