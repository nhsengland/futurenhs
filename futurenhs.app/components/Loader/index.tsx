import { useRef, useState, useEffect } from 'react';
import classNames from 'classnames'

import { Spinner } from '@components/Spinner';

import { Props } from './interfaces'

export const Loader: (props: Props) => JSX.Element = ({
    delay = 0,
    className,
}) => {

    const [shouldRender, setShouldRender] = useState(false);
    const loadingTimeOut: any = useRef(null);

    const generatedClasses: any = {
        wrapper: classNames('c-loader', className),
        spinner: classNames(`u-w-[180px] u-h-[180px]`)
    };

    useEffect(() => {

        loadingTimeOut.current = window.setTimeout(() => {

            setShouldRender(true);

        }, delay);

        return () => {

            window.clearTimeout(loadingTimeOut.current);

        }

    }, []);

    if(shouldRender){

        return (
            <div className={generatedClasses.wrapper}>
                <div className={generatedClasses.spinner}>
                    <Spinner />
                </div>
            </div>
        )

    }

    return null

}
