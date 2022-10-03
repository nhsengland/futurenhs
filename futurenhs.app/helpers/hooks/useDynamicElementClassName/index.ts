import { useEffect, useRef } from 'react'

export const useDynamicElementClassName = (config: {
    elementSelector: string
    addClass?: string
    removeClass?: string
}): boolean => {
    const element = useRef(null)
    const originalElement = useRef(null)

    const { elementSelector, addClass, removeClass } = config

    useEffect(() => {
        element.current = document.querySelector(elementSelector)
        originalElement.current = element.current?.cloneNode()

        const classToRemove: Array<string> = removeClass?.split(' ')
        const classToAdd: Array<string> = addClass?.split(' ')

        element.current?.classList.remove(...classToRemove)
        element.current?.classList.add(...classToAdd)

        return () => {
            element.current?.classList.forEach((className) =>
                element.current.classList.remove(className)
            )
            originalElement.current?.classList.forEach((className) =>
                element.current.classList.add(className)
            )
        }
    }, [elementSelector])

    return null
}
