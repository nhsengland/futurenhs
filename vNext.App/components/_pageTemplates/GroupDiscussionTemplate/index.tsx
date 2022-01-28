import { useState } from 'react';
import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { Link } from '@components/Link';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { RichText } from '@components/RichText';
import { LayoutColumn } from '@components/LayoutColumn';
import { BackLink } from '@components/BackLink';
import { Card } from '@components/Card';
import { SVGIcon } from '@components/SVGIcon';
import { getGroupDiscussion } from '@services/getGroupDiscussion';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';

import { Props } from './interfaces';

export const GroupDiscussionTemplate: (props: Props) => JSX.Element = ({
    groupId,
    user,
    contentText,
    entityText,
    image,
    actions,
    discussion
}) => {

    const router = useRouter();

    const backLinkHref: string = getRouteToParam({
        router: router,
        paramName: routeParams.DISCUSSIONID
    });

    const { text: discussionText } = discussion ?? {};
    const { title, body } = discussionText ?? {};

    return (

        <GroupLayout 
            id="forum"
            user={user}
            actions={actions}
            text={entityText}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn tablet={8} className="c-page-body">
                    <BackLink 
                        href={backLinkHref}
                        text={{
                            link: "Back to discussions"
                        }} />
                    <h2>{title}</h2>
                    {body &&
                        <RichText bodyHtml={body} />
                    }
                </LayoutColumn>
        </GroupLayout>

    )

}
