import classNames from 'classnames';
import { Avatar } from '@components/Avatar';

import { Props } from './interfaces';

export const UserMeta: (props: Props) => JSX.Element = ({
    image,
    text,
    children,
    className
}) => {

    const { initials } = text;

    const generatedClasses = {
        wrapper: classNames('u-flex', className),
        avatar: classNames('u-block', 'u-h-12', 'u-w-12', 'u-min-w-fit', 'u-mr-4')
    }

    return (

        <p className={generatedClasses.wrapper}>
            <Avatar image={image} initials={initials} className={generatedClasses.avatar} />
            <span className="u-m-0">
                {children}
            </span>
        </p>

    );

}
