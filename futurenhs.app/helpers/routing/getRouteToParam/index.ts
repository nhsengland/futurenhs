declare interface Config {
    router: any
    paramName: string
    shouldIncludeParam?: boolean
}

export const getRouteToParam = ({
    router,
    paramName,
    shouldIncludeParam,
}: Config): string => {
    try {
        const currentPathName: string = router?.asPath
        const paramValue: string = router?.query[paramName]

        if (!paramValue) {
            return ''
        }

        const endSliceIndex: number =
            currentPathName.indexOf(paramValue) +
            (shouldIncludeParam ? paramValue.length : -1)
        const route: string = currentPathName?.slice(0, endSliceIndex)

        return route
    } catch (error) {
        console.error(error)

        return ''
    }
}

export const getRouteToParam2 = ({
    route,
    param,
    shouldIncludeParam = true,
}: any): string => {
    if (!route || !param) {
        return undefined
    }

    try {
        const endSliceIndex: number =
            route.indexOf(param) + (shouldIncludeParam ? param.length : -1)
        const formattedRoute: string = route?.slice(0, endSliceIndex)

        return formattedRoute
    } catch (error) {
        return ''
    }
}
