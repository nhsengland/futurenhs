import classNames from 'classnames';

import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { Pagination } from '@components/Pagination';
import { PaginationStatus } from '@components/PaginationStatus';

import { Props } from './interfaces';

export const PaginationWithStatus: (props: Props) => JSX.Element = ({
    id,
    text,
    visiblePages = 5,
    pageNumber,
    pageSize,
    totalRecords,
    shouldEnableLoadMore,
    shouldDisable,
    getPageAction,
    className
}) => {

    const generatedClasses = {
        wrapper: classNames('u-mt-12', 'u-flex-col', 'tablet:u-flex-row-reverse', className),
        status: classNames('u-text-right', 'u-self-center', 'u-mb-4', 'tablet:u-mb-0'),
        controls: classNames('')
    }

    if(totalRecords <= pageSize){

        return null;

    }

    return (

        <LayoutColumnContainer className={generatedClasses.wrapper}>
            <LayoutColumn tablet={6} className={generatedClasses.status}>
                <PaginationStatus 
                    shouldEnableLoadMore={shouldEnableLoadMore} 
                    pageNumber={pageNumber}
                    pageSize={pageSize}
                    totalRecords={totalRecords} />
            </LayoutColumn>
            <LayoutColumn tablet={6} className={generatedClasses.controls}>
                <Pagination 
                    id={id}
                    text={text} 
                    shouldEnableLoadMore={shouldEnableLoadMore}
                    shouldDisable={shouldDisable}
                    pageNumber={pageNumber}
                    pageSize={pageSize}
                    totalRecords={totalRecords}
                    visiblePages={visiblePages}
                    getPageAction={getPageAction} />
            </LayoutColumn>
        </LayoutColumnContainer>

    )

}
