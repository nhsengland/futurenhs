
import { useState, useEffect } from 'react';
import { Props } from './interfaces';

export const PaginationStatus: (props: Props) => JSX.Element = ({
    content = {
        prefixText: 'Showing',
        infixText: 'of',
        suffixText: 'items'
    },
    shouldEnableLoadMore,
    pageNumber,
    pageSize,
    totalRecords
}) => {

    if(!pageNumber || !pageSize || !totalRecords){

        return null;

    }

    const [isLoadMoreEnabled, setIsLoadMoreEnabled] = useState(false);
    
    const { prefixText, infixText, suffixText } = content;

    const currentEnd: number = pageNumber * pageSize;
    const start: number = pageNumber === 1 || isLoadMoreEnabled ? 1 : ((pageNumber - 1) * pageSize) + 1;
    const end: number = currentEnd < totalRecords ? currentEnd : totalRecords;

    useEffect(() => {

        setIsLoadMoreEnabled(shouldEnableLoadMore);

    }, [shouldEnableLoadMore]);

    return (

        <p className="c-pagination-status"> 
            <span className="u-text-bold">{`${prefixText} ${start} - ${end}`}</span> {`${infixText}`} <span className="u-text-bold">{totalRecords}</span> {`${suffixText}`}   
        </p>

    )

}
