import classNames from 'classnames';

import { Link } from '@components/Link';

import { Props } from './interfaces';

export const TabbedNav: (props: Props) => JSX.Element = ({
    text,
    navMenuList
}) => {

    const { ariaLabel } = text;

    const generatedClasses: any = {
        wrapper: classNames('c-tabbed-nav'),
        item: classNames('c-tabbed-nav_item')
    }; 

    return (

        <nav className="c-tabbed-nav_nav" aria-label={ariaLabel}>
            <ul role="menu" className="u-list-plain c-tabbed-nav_list">
                {navMenuList.map(({ 
                    url, 
                    text,
                    isActive 
                }, index) => {
                    
                    generatedClasses.link = classNames('c-tabbed-nav_link', {
                        ['c-tabbed-nav_link--active u-hidden tablet:u-block']: isActive
                    }); 

                    return (

                        <li key={index} role="none" className={generatedClasses.item}>
                            <Link href={url}>
                                <a role="menuitem" aria-current={isActive} className={generatedClasses.link}>
                                    <span>{text}</span>
                                </a>
                            </Link>
                        </li>

                    )

                })}
            </ul>
        </nav>

    )

}
