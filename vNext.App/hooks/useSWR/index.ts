import useSWR from 'swr';

export const useService = ({
    key,
    swrOptions,
    fetcher,
    fetcherOptions
}): {
    data: any;
    error: string;
    mutate?: any;
} => {

    const { data, error, mutate } = useSWR(key, async () => {
        
        const { data, errors } = await fetcher(fetcherOptions);

        return {
            data: data,
            error: errors && Object.keys(errors).length ? Object.keys(errors)[1] : null
        }

    }, swrOptions);
    
    return {
        data: data,
        error: error,
        mutate: mutate
    }

};