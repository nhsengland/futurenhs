import { useRef, useState, useEffect } from 'react';
import classNames from 'classnames'

import { Spinner } from '@components/Spinner';

import { Props } from './interfaces'

export const Loader: (props: Props) => JSX.Element = ({
    isActive,
    text,
    delay = 0,
    remain = 0,
    className,
}) => {

    const [shouldRender, setShouldRender] = useState(false);
    const [isEnding, setIsEnding] = useState(false);
    const loadingTimeOut: any = useRef(null);
    const endingTimeOut: any = useRef(null)

    const { loadingMessage } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-loader', className, {
            [`u-transition-opacity u-duration-750 u-ease-out u-opacity-0`]: isEnding
        }),
        content: classNames('c-loader_content'),
        message: classNames('c-loader_message'),
        spinner: classNames('c-loader_spinner')
    };

    const renderAfterDelay = (): void => {

        window.clearTimeout(endingTimeOut.current)
        setIsEnding(false)

        loadingTimeOut.current = window.setTimeout(() => {

            isActive && setShouldRender(true);
            
        }, delay);

    }


    const hideAfterRemain = (): void => {

        setIsEnding(true)

        endingTimeOut.current = window.setTimeout(() => {

            setShouldRender(false)

        }, remain);

    }

    useEffect(() => {

        isActive ? renderAfterDelay() : hideAfterRemain()

        return () => {

            window.clearTimeout(loadingTimeOut.current);
            window.clearTimeout(endingTimeOut.current);

        }

    }, [isActive]);


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
