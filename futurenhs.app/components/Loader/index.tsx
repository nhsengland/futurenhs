import { useRef, useState, useEffect } from 'react';
import classNames from 'classnames'

import { Spinner } from '@components/Spinner';

import { Props } from './interfaces'

export const Loader: (props: Props) => JSX.Element = ({
    text,
    delay = 0,
    className,
}) => {

    const [shouldRender, setShouldRender] = useState(false);
    const loadingTimeOut: any = useRef(null);

    const { loadingMessage } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-loader', className),
        content: classNames('c-loader_content'),
        message: classNames('c-loader_message'),
        spinner: classNames('c-loader_spinner')
    };

    const renderAfterDelay = (): void => {

        loadingTimeOut.current = window.setTimeout(() => {

            setShouldRender(true);

        }, delay);

    }

    useEffect(() => {

        renderAfterDelay();

        return () => {

            window.clearTimeout(loadingTimeOut.current);

        }

    }, []);

    useEffect(() => {

        setShouldRender(false);
        renderAfterDelay();

    }, [delay]);

    if(shouldRender){

        return (
            <div className={generatedClasses.wrapper}>
                <div className={generatedClasses.content}>
                    <p className={generatedClasses.message}>{loadingMessage}</p>
                    <Spinner className={generatedClasses.spinner} />
                </div>
            </div>
        )

    }

    return null

}
