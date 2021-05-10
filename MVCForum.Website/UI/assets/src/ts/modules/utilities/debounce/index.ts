export function debounceTimout(callback: Function, wait: number) {

    let timeout: number;

    return (...args) => {

        const later: Function = () => {

            timeout = null;
            callback(...args);

        };

        clearTimeout(timeout);

        timeout = setTimeout(later, wait);

    };
}