import classNames from 'classnames'

import { RichText } from '@components/RichText'

import { Props } from './interfaces'

export const GroupContainer: (props: Props) => JSX.Element = ({
    children,
    className,
}) => {
    const generatedClasses: any = {
        wrapper: classNames(className),
    }

    return <div className={generatedClasses.wrapper}>{children}</div>
}
