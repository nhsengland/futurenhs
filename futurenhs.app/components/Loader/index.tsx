import classNames from 'classnames'

import { Spinner } from '@components/Spinner';

import { Props } from './interfaces'

export const Loader: (props: Props) => JSX.Element = ({
    className,
}) => {

    const generatedClasses: any = {
        wrapper: classNames('u-fixed u-w-full u-h-full u-z-50 u-flex u-justify-center u-items-center', className),
        spinner: classNames(`u-w-[180px] u-h-[180px]`)
    };

    return (
        <div className={generatedClasses.wrapper}>
            <div className={generatedClasses.spinner}>
                <Spinner />
            </div>
        </div>
    )

}
