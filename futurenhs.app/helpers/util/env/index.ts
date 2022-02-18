/**
 * Evaluates whether a property is defined and not null
 */
 export const isDefined = (property: any): boolean => {

    return typeof property !== 'undefined' && property !== null && property !== '';

}

/**
 * Gets an environment variable and throws if not available
 */
export const getEnvVar = ({
    name,
    isRequired = true
}: {
    name: string;
    isRequired?: boolean
}): string | number => {

    const value: string | number = process.env[name];

    if(!isDefined(value) && isRequired){

        throw new Error(`Environment variable ${name} is not defined`);

    }

    return value;

};
