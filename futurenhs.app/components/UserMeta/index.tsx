import classNames from 'classnames'
import { Avatar } from '@components/Avatar'

import { Props } from './interfaces'

export const UserMeta: (props: Props) => JSX.Element = ({
    image,
    text,
    children,
    className,
}) => {
    const { initials } = text

    const generatedClasses = {
        wrapper: classNames('c-user-meta', className),
        avatar: classNames('c-user-meta_avatar', 'u-h-12', 'u-w-12'),
        body: classNames('c-user-meta_body'),
    }

    return (
        <p className={generatedClasses.wrapper}>
            <Avatar
                image={image}
                initials={initials}
                className={generatedClasses.avatar}
            />
            <span className={generatedClasses.body}>{children}</span>
        </p>
    )
}
