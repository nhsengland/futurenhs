import classNames from 'classnames';

import { Link } from '@components/Link';
import { SVGIcon } from '@components/SVGIcon';

import { Props } from './interfaces';

export const BackLink: (props: Props) => JSX.Element = ({
    href,
    content,
    className
}) => {

    const { linkText } = content ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-back-link', className),
        icon: classNames('c-back-link_icon')
    };

    return (

        <p>
            <Link href={href}>
                <a className={generatedClasses.wrapper}>
                    <SVGIcon name="icon-chevron-left" className={generatedClasses.icon} />
                    {linkText}
                </a>
            </Link>
        </p>

    )

}
