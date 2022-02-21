import classNames from 'classnames';

import { Link } from '@components/Link';
import { SVGIcon } from '@components/SVGIcon';

import { Props } from './interfaces';

export const MainNav: (props: Props) => JSX.Element =  ({
    navMenuList,
    className
}) => {

    const generatedClasses: any = {
        wrapper: classNames('c-main-nav', 'u-bg-theme-2', className),
        nav: classNames('c-main-nav_level-1')
    };

    return (

        <div className={generatedClasses.wrapper}>
            <nav id="main-nav" aria-label="Main">
                <ul className={generatedClasses.nav} role="menubar">
                    {navMenuList?.map(({ url, text, isActive, meta }, index) => {

                        const { themeId, iconName } = meta ?? {};

                        const generatedClasses = {
                            link: classNames('c-main-nav_level-1-link', {
                                [`c-main-nav_level-1-link--active`]: isActive,
                                [`u-border-theme-${themeId}`]: isActive && typeof themeId !== 'undefined',
                                [`u-text-theme-${themeId}`]: typeof themeId !== 'undefined'
                            }),
                            icon: classNames('c-main-nav_level-1-icon', {
                                [`u-fill-theme-${themeId}`]: typeof themeId !== 'undefined'
                            })
                        }

                        return (

                            <li key={index} role="none">
                                <Link href={url}>
                                    <a role="menuitem" aria-current={isActive} aria-label={text} title={text} className={generatedClasses.link}>
                                        {iconName &&
                                            <SVGIcon name={iconName} className={generatedClasses.icon} />
                                        }
                                    </a>
                                </Link>
                            </li>

                        )

                    })}
                </ul>
            </nav>
        </div>

    )

}
