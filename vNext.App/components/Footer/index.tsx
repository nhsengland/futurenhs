import classNames from 'classnames';

import { Link } from '@components/Link';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { LayoutWidthContainer } from '@components/LayoutWidthContainer';

import { Props } from './interfaces';

export const Footer: (props: Props) => JSX.Element = ({
    content,
    navMenuList,
    className
}) => {

    const { titleText, 
            navMenuAriaLabelText, 
            copyrightText } = content ?? {};

    const generatedClasses: any = {
        wrapper: classNames('c-site-footer', className),
        heading: classNames('u-sr-only'),
        nav: classNames('c-site-footer_nav'),
        navItem: classNames('c-site-footer_nav-item'),
        copyRight: classNames('c-site-footer_copyright')
    };

    return (

        <footer className={generatedClasses.wrapper}>
            <h2 className={generatedClasses.heading}>{titleText}</h2>
            <div className={generatedClasses.nav}>
                <LayoutWidthContainer>
                    <LayoutColumnContainer>
                        <LayoutColumn desktop={10}>
                            <nav aria-label={navMenuAriaLabelText}>
                                <ul role="menu" className="u-list-none u-m-0 u-p-0">
                                    {navMenuList.map(({ url, text, isActive }, index) => {

                                        return (

                                            <li key={index} role="none" className={generatedClasses.navItem}>
                                                <Link href={url}>
                                                    <a role="menuitem" rel="noopener" aria-current={isActive}>{text}</a>
                                                </Link>
                                            </li>

                                        )

                                    })}
                                </ul>
                            </nav>
                        </LayoutColumn>
                        {copyrightText &&
                            <LayoutColumn desktop={2}>
                                <p className={generatedClasses.copyRight}>
                                    <span dangerouslySetInnerHTML={{ "__html": "&copy;" }} /> {copyrightText}
                                </p>
                            </LayoutColumn>
                        }
                    </LayoutColumnContainer>
                </LayoutWidthContainer>
            </div>
        </footer >

    )

}
