using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PathFind : MonoBehaviour
{
    [SerializeField] private Sprite circleImage;
    [SerializeField] private TMP_Dropdown dropdownBox;
    [SerializeField] private LineRenderer pathLinePrefab;
    private LineRenderer pathLine;
    public List<Star> path = new List<Star>();

    //Create the path line renderer.
    private void Awake() {
        pathLine = Instantiate(pathLinePrefab);
    }

    //Run Dijkstra's
    public void StartGetPath(Star start, Star end) {
        path = GetPath(start, end);
        DrawPath();
    }
    
    //Use the found path to create an array of points for the line renderer.
    private void DrawPath() {
        dropdownBox.ClearOptions();
        List<TMP_Dropdown.OptionData> newOptionData = new List<TMP_Dropdown.OptionData>();
        newOptionData.Add(new TMP_Dropdown.OptionData("Select Course Planet"));
        Vector3[] points = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++) {
            points[i] = path[i].transform.position;
            newOptionData.Add(new TMP_Dropdown.OptionData(path[i].name, circleImage));
        }
        pathLine.positionCount = path.Count;
        pathLine.SetPositions(points);
        dropdownBox.AddOptions(newOptionData);
    }

    //Dijkstra's
    public List<Star> GetPath(Star start, Star end) {
        List<StarPath> priority = new List<StarPath>();
        foreach (Star star in GetComponent<StarSpawner>().allStars) {
            StarPath newPath = new StarPath(star, null);
            if (star == start) {
                newPath.shortestToStart = newPath;
                newPath.smallestToStart = 0.0f;
            }
            priority.Add(newPath);
        }
        priority = OrderPriorityListByDistance(priority);
        while (priority.Count > 0) {
            StarPath currentStarNode = priority[0];
            foreach (StarPath nextStar in GetStarPathsFromRoutesDictionary(currentStarNode.star.starRoutes, priority)) {
                float distance = nextStar.star.starRoutes[currentStarNode.star] + currentStarNode.smallestToStart;
                if (distance < nextStar.smallestToStart) {
                    nextStar.smallestToStart = distance;
                    nextStar.shortestToStart = currentStarNode;
                }
            }
            priority.Remove(currentStarNode);
            if (priority.Count > 0) {
                priority = OrderPriorityListByDistance(priority);
                if (priority[0].star == end && priority[0].smallestToStart != float.MaxValue) {
                    List<Star> pathToEnd = new List<Star>();
                    StarPath backtrackStar = priority[0];
                    pathToEnd.Add(backtrackStar.star);
                    while (backtrackStar.star != start) {
                        backtrackStar = backtrackStar.shortestToStart;
                        pathToEnd.Add(backtrackStar.star);
                    }
                    pathToEnd.Reverse();
                    return pathToEnd;
                }
            }
        }
        return new List<Star>();
    }

    private static List<StarPath> OrderPriorityListByDistance(List<StarPath> apathList) {
        for (int i = 0; i < apathList.Count; i++) {
            for (int j = 0; j < apathList.Count - 1; j++) {
                StarPath first = apathList[j];
                StarPath second = apathList[j + 1];
                if (first.smallestToStart > second.smallestToStart) {
                    apathList[j] = second;
                    apathList[j + 1] = first;
                }
            }
        }
        return apathList;
    }

    private static List<StarPath> GetStarPathsFromRoutesDictionary(Dictionary<Star, float> starRoutes, List<StarPath> priorityList) {
        List<StarPath> result = new List<StarPath>();

        foreach (KeyValuePair<Star, float> route in starRoutes) {
            Star star = route.Key;
            for (int i = 0; i < priorityList.Count; i++) {
                if (priorityList[i].star == star) {
                    result.Add(priorityList[i]);
                    break;
                }
            }
        }
        return result;
    }

    public class StarPath {
        public Star star;
        public StarPath shortestToStart;
        public float smallestToStart;

        public StarPath(Star current, StarPath shortest) {
            star = current;
            shortestToStart = shortest;
            smallestToStart = float.MaxValue;
        }
    }
}
