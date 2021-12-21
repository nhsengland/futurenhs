import { RichText } from '@components/RichText';

import { Props } from './interfaces';

export const BreadCrumb: (props: Props) => JSX.Element = ({
    content,
    navMenuList
}) => {

    const { descriptionHtml } = content ?? {};

    return (

        <div className="c-breadcrumb"> 
            <div className="c-breadcrumb_path">
                
            </div>
            {descriptionHtml &&
                <div className="c-breadcrumb_support-disclaimer">
                    <RichText 
                        wrapperElementType="p" 
                        bodyHtml={descriptionHtml} 
                        className="u-mb-0" />
                </div>
            }   
        </div>

    )

}
