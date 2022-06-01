import { useState, useEffect, useRef } from 'react'
import classNames from 'classnames'

import { Props } from './interfaces'

export const Loading: (props: Props) => JSX.Element = ({
    timeOut = 800,
    className,
}) => {

    const timeOutHandler = useRef(null);
    const [shouldRender, setShouldRender] = useState(false);

    const generatedClasses: any = {
        wrapper: classNames('u-fixed u-w-full u-h-full u-z-50 u-flex u-justify-center u-items-center', className),
        loader: classNames('u-absolute u-block u-w-[160px] u-h-[160px] u-bg-theme-1 u-overflow-hidden u-rounded-full u-shadow-md u-p-4 u-opacity-90')
    };

    useEffect(() => {

        timeOutHandler.current = window.setTimeout(() => {

            setShouldRender(true);

        }, timeOut);

        return () => {

            timeOutHandler.current = null;

        }

    }, []);

    if(shouldRender){

        return (
            <div className={generatedClasses.wrapper}>
                <div className={generatedClasses.loader}>
                    <img src="/images/loading.gif" className="u-w-full u-h-full" alt="Loading" />
                </div>
            </div>
        )

    }

    return null

}

export default Loading;
