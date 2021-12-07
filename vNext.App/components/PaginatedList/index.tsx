import { Pagination } from '@components/Pagination';

import { Props } from './interfaces';

export const PaginatedList: (props) => JSX.Element = ({
    pageSize,
    totalPages,
    children
}) => {

    const hasPagination: boolean = totalPages > 1;
    
    return (

        <> 
            {children}
            {hasPagination &&
                <Pagination />
            }   
        </>

    )

}
