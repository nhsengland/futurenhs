import classNames from 'classnames';

import { Props } from './interfaces';

export const PageBody: (props: Props) => JSX.Element = ({
    children,
    className
}) => {

    const generatedClasses: any = {
        wrapper: classNames('c-page-body', 'u-w-full', 'tablet:u-px-4', 'u-py-10', className)
    };

    return (

        <div className={generatedClasses.wrapper}>
            {children}
        </div>

    )

}
