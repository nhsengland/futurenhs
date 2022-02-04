import useSWR from 'swr';

export const useService = ({
    key,
    options,
    service
}): {
    data?: any;
    errors?: Array<string>;
    isProcessing?: boolean;
} => {

    try {

        const { data } = useSWR(key, service().then(({ data, errors }) => {

            if(errors?.length){ 
                
                throw new Error(errors[0]);
            
            }
    
            return data;
    
        }), options);

        return {
            data: data,
            errors: null,
            isProcessing: !data
        }

    } catch(error){

        return {
            data: null,
            errors: [error],
            isProcessing: false
        }

    }

};

// WIP
// const { data, error } = useSWR(pagination.pageNumber.toString(), () => getGroupDiscussionComments({
//     groupId,
//     discussionId,
//     user,
//     pagination
// }).then(({ data, errors }) => {

//     if(errors?.length){

//         throw new Error(errors[0]);

//     }

//     return data;

// }), 
// {
//     refreshInterval: 10000,
//     fallbackData: discussionComments,
//     revalidateOnMount: false
// });