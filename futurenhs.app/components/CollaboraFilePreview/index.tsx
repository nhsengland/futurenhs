import { useRef, useEffect } from 'react'
import classNames from 'classnames'

import { requestMethods } from '@constants/fetch'

import { Props } from './interfaces'

export const CollaboraFilePreview: (props: Props) => JSX.Element = ({
    csrfToken,
    accessToken,
    wopiClientUrl,
    className,
}) => {
    const initFormRef: any = useRef()
    const iFrameName: string = 'collabora-online-viewer'

    const generatedClasses: any = {
        wrapper: classNames(className),
        form: classNames('u-hidden'),
        iFrame: classNames('u-w-full', 'u-min-h-screen'),
    }

    useEffect(() => {
        initFormRef.current?.submit()
    }, [])

    return (
        <div className={generatedClasses.wrapper}>
            <form
                ref={initFormRef}
                name="collabora-init"
                encType="multipart/form-data"
                action={wopiClientUrl}
                method={requestMethods.POST}
                target={iFrameName}
                className={generatedClasses.form}
            >
                <input name="_csrf" value={csrfToken} type="hidden" />
                <input name="access_token" value={accessToken} type="hidden" />
                <input type="submit" value="" />
            </form>
            <iframe
                id={iFrameName}
                name={iFrameName}
                className={generatedClasses.iFrame}
            ></iframe>
        </div>
    )
}
