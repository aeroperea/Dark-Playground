using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public QuestContainer questContainer;
    public List<Quest> questTemplates;
    private List<QuestInstance> activeQuests;

    public static QuestManager Instance { get; private set; }

    public List<QuestInstance> ActiveQuests => activeQuests;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            activeQuests = new List<QuestInstance>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        EventManager.StartListening(GameEventType.ItemCollected, OnItemCollected);
        EventManager.StartListening(GameEventType.EnemyDefeated, OnEnemyDefeated);
        EventManager.StartListening(GameEventType.LocationReached, OnLocationReached);
        EventManager.StartListening(GameEventType.DialogueNodePassed, OnDialogueNodePassed);
        EventManager.StartListening(GameEventType.PlayerResponseSelected, OnPlayerResponseSelected);
        EventManager.StartListening(GameEventType.EscortTargetReached, OnEscortTargetReached);
    }

    private void UnsubscribeFromEvents()
    {
        EventManager.StopListening(GameEventType.ItemCollected, OnItemCollected);
        EventManager.StopListening(GameEventType.EnemyDefeated, OnEnemyDefeated);
        EventManager.StopListening(GameEventType.LocationReached, OnLocationReached);
        EventManager.StopListening(GameEventType.DialogueNodePassed, OnDialogueNodePassed);
        EventManager.StopListening(GameEventType.PlayerResponseSelected, OnPlayerResponseSelected);
        EventManager.StopListening(GameEventType.EscortTargetReached, OnEscortTargetReached);
    }

    public void LoadQuest(string questName)
    {
        questContainer = Resources.Load<QuestContainer>(questName);
        if (questContainer == null)
        {
            Debug.LogError("QuestContainer not found!");
            return;
        }

        InitializeQuests();
    }

    private void InitializeQuests()
    {
        foreach (var nodeData in questContainer.questNodeData)
        {
            var quest = new QuestInstance(new Quest { questName = nodeData.questName, questDescription = nodeData.questDescription });
            QuestInstanceObjective lastObjective = null;
            foreach (var link in questContainer.nodeLinks.Where(l => l.baseNodeGuid == nodeData.GUID))
            {
                var targetObjectiveData = questContainer.objectiveNodeData.FirstOrDefault(o => o.GUID == link.targetNodeGuid);
                if (targetObjectiveData != null)
                {
                    var newObjective = new QuestInstanceObjective(new QuestObjective(targetObjectiveData.objectiveType)
                    {
                        description = targetObjectiveData.objectiveDescription,
                        objectiveType = targetObjectiveData.objectiveType,
                        isOptional = targetObjectiveData.isOptional,
                        targetId = targetObjectiveData.targetId,
                        status = ObjectiveStatus.Hidden,
                        requiredItemCount = targetObjectiveData.objectiveType == ObjectiveType.Item ? 1 : 0,
                        currentItemCount = 0,
                        location = targetObjectiveData.targetTransform != null ? targetObjectiveData.targetTransform.position : Vector3.zero
                    });
                    if (lastObjective != null)
                    {
                        lastObjective.nextObjective = newObjective; // Linking objectives
                    }
                    lastObjective = newObjective;
                    quest.objectives.Add(newObjective);

                    // Assign Triggers for this Objective
                    AssignTriggers(newObjective, quest);
                }
            }
            activeQuests.Add(quest);
            quest.RevealNextObjective();
        }
    }

    private void AssignTriggers(QuestInstanceObjective objective, QuestInstance quest)
    {
        var triggers = FindObjectsOfType<QuestTrigger>();
        foreach (var trigger in triggers)
        {
            print(trigger.targetId + " help " + objective.targetId);
            if (trigger.targetId == objective.targetId) // Ensure your triggers have a targetId field that matches the objective's targetId
            {
                trigger.questInstance = quest;
                trigger.objectiveIndex = quest.objectives.IndexOf(objective);
                trigger.SubscribeToObjectiveStatus(); // This method should manage trigger activation based on objective status
            }
        }
    }

    public void UpdateQuest(string questName)
    {
        var quest = activeQuests.FirstOrDefault(q => q.questName == questName);
        if (quest != null)
        {
            if (quest.CheckObjectives())
            {
                Debug.Log($"{quest.questName} is completed!");
                GameManager.Instance.UpdateObjectiveText("");

                // If there is a next quest linked, start it
                if (!string.IsNullOrEmpty(quest.nextQuestGUID))
                {
                    var nextQuestData = questContainer.questNodeData.FirstOrDefault(q => q.GUID == quest.nextQuestGUID);
                    if (nextQuestData != null)
                    {
                        StartNextQuest(nextQuestData);
                    }
                }
                else
                {
                    GameManager.Instance.UpdateObjectiveText("");
                }
            }
            else
            {
                UpdateObjectiveText(quest);
            }
        }
    }

    private void StartNextQuest(QuestNodeData nextQuestData)
    {
        var quest = new QuestInstance(new Quest { questName = nextQuestData.questName, questDescription = nextQuestData.questDescription });
        foreach (var link in questContainer.nodeLinks)
        {
            if (link.baseNodeGuid == nextQuestData.GUID)
            {
                var targetObjectiveData = questContainer.objectiveNodeData.FirstOrDefault(o => o.GUID == link.targetNodeGuid);
                if (targetObjectiveData != null)
                {
                    quest.objectives.Add(new QuestInstanceObjective(new QuestObjective(targetObjectiveData.objectiveType)
                    {
                        description = targetObjectiveData.objectiveDescription,
                        objectiveType = targetObjectiveData.objectiveType,
                        isOptional = targetObjectiveData.isOptional,
                        targetId = targetObjectiveData.targetId,
                        status = ObjectiveStatus.Hidden,
                        requiredItemCount = targetObjectiveData.objectiveType == ObjectiveType.Item ? 1 : 0,
                        currentItemCount = 0,
                        location = targetObjectiveData.targetTransform != null ? targetObjectiveData.targetTransform.position : Vector3.zero // Use transform position
                    }));
                }

                var targetQuestData = questContainer.questNodeData.FirstOrDefault(q => q.GUID == link.targetNodeGuid);
                if (targetQuestData != null)
                {
                    quest.nextQuestGUID = targetQuestData.GUID;
                }
            }
        }

        activeQuests.Add(quest);
        quest.RevealNextObjective();
        UpdateObjectiveText(quest);
    }

    public void AddQuest(Quest questTemplate)
    {
        var quest = new QuestInstance(questTemplate);
        if(activeQuests == null)
        {
            activeQuests = new List<QuestInstance>();
        }
        activeQuests.Add(quest);
        quest.RevealNextObjective();
        SetupQuestObjectives(quest);
        UpdateObjectiveText(quest);
    }

    private void SetupQuestObjectives(QuestInstance quest)
    {
        foreach (var objective in quest.objectives)
        {
            AssignTriggers(objective, quest);
        }
    }

    public QuestInstance GetQuest(string questName)
    {
        return activeQuests.FirstOrDefault(quest => quest.questName == questName);
    }

    private void OnItemCollected(GameEvent gameEvent)
    {
        HandleEvent(gameEvent, ObjectiveType.Item);
    }

    private void OnEnemyDefeated(GameEvent gameEvent)
    {
        HandleEvent(gameEvent, ObjectiveType.Kill);
    }

    private void OnLocationReached(GameEvent gameEvent)
    {
        Vector3 location = (Vector3)gameEvent.EventData;
        foreach (var quest in activeQuests)
        {
            foreach (var objective in quest.objectives)
            {
                if (objective.objectiveType == ObjectiveType.Location && objective.location == location && objective.status == ObjectiveStatus.Active)
                {
                    CompleteObjective(quest, objective);
                }
            }
        }
    }

    private void OnDialogueNodePassed(GameEvent gameEvent)
    {
        HandleEvent(gameEvent, ObjectiveType.DialogueNode);
    }

    private void OnPlayerResponseSelected(GameEvent gameEvent)
    {
        HandleEvent(gameEvent, ObjectiveType.DialogueNode);
    }

    private void OnEscortTargetReached(GameEvent gameEvent)
    {
        HandleEvent(gameEvent, ObjectiveType.Escort);
    }

    private void HandleEvent(GameEvent gameEvent, ObjectiveType targetType)
    {
        string targetId = gameEvent.EventData as string;
        foreach (var quest in activeQuests)
        {
            foreach (var objective in quest.objectives)
            {
                if (objective.objectiveType == targetType && objective.targetId == targetId && objective.status == ObjectiveStatus.Active)
                {
                    if (targetType == ObjectiveType.Item)
                    {
                        objective.currentItemCount++;
                        if (objective.currentItemCount >= objective.requiredItemCount)
                        {
                            CompleteObjective(quest, objective);
                        }
                        else
                        {
                            UpdateObjectiveText(quest);
                        }
                    }
                    else
                    {
                        CompleteObjective(quest, objective);
                    }
                }
            }
        }
    }

    private void HandleObjectiveStatusChange(QuestInstanceObjective objective, ObjectiveStatus status)
    {
        // Depending on the game design, you may need references to triggers or other components that respond to the objective status
        switch (status)
        {
            case ObjectiveStatus.Active:
                // Activate the trigger or UI related to this objective
                ActivateTrigger(objective);
                break;
            case ObjectiveStatus.Completed:
                // Deactivate the trigger or update UI as the objective is completed
                DeactivateTrigger(objective);
                break;
            case ObjectiveStatus.Skipped:
                // Handle skipping of the objective, possibly deactivating or ignoring triggers
                DeactivateTrigger(objective);
                break;
            default:
                // For hidden or other statuses, ensure that triggers are not active
                DeactivateTrigger(objective);
                break;
        }
    }

    private void ActivateTrigger(QuestInstanceObjective objective)
    {
        // Logic to find and activate the game object or UI element linked to this objective
        // For example, enabling a collider or making an interactable object visible
        GameObject triggerObject = FindTriggerObjectByObjectiveId(objective.targetId);
        if (triggerObject != null)
        {
            triggerObject.SetActive(true);
        }
    }

    private void DeactivateTrigger(QuestInstanceObjective objective)
    {
        // Logic to find and deactivate the game object or UI element linked to this objective
        // For example, disabling a collider or making an interactable object non-visible
        GameObject triggerObject = FindTriggerObjectByObjectiveId(objective.targetId);
        if (triggerObject != null)
        {
            triggerObject.SetActive(false);
        }
    }

    private GameObject FindTriggerObjectByObjectiveId(string targetId)
    {
        // Implement logic to find the appropriate game object based on the targetId of the objective
        // This might involve searching through a list or using a dictionary if you keep a registry of triggers
        return GameObject.Find(targetId); // Example using GameObject.Find which you should avoid in production
    }


    public void CompleteObjective(QuestInstance quest, QuestInstanceObjective objective)
    {
        objective.status = ObjectiveStatus.Completed;
        if (quest.CheckObjectives())
        {
            Debug.Log($"{quest.questName} is completed!");
            GameManager.Instance.UpdateObjectiveText("");

            // If there is a next quest linked, start it
            if (!string.IsNullOrEmpty(quest.nextQuestGUID))
            {
                var nextQuestData = questContainer.questNodeData.FirstOrDefault(q => q.GUID == quest.nextQuestGUID);
                if (nextQuestData != null)
                {
                    StartNextQuest(nextQuestData);
                }
            }
        }
        else
        {
            quest.SkipOptionalObjectives();
            quest.RevealNextObjective();
            UpdateObjectiveText(quest);
        }
    }


    private void UpdateObjectiveText(QuestInstance quest)
    {
        if (quest.status == QuestStatus.Completed)
        {
            GameManager.Instance.UpdateObjectiveText($"{quest.questName} - Completed!");
        }
        else
        {
            QuestInstanceObjective nextObjective = quest.objectives.FirstOrDefault(o => o.status == ObjectiveStatus.Active);
            if (nextObjective != null)
            {
                if (nextObjective.objectiveType == ObjectiveType.Item)
                {
                    GameManager.Instance.UpdateObjectiveText($"{quest.questName} - {nextObjective.description} ({nextObjective.currentItemCount}/{nextObjective.requiredItemCount})");
                }
                else
                {
                    GameManager.Instance.UpdateObjectiveText($"{quest.questName} - {nextObjective.description}");
                }
            }
        }
    }

    public void SetObjectiveVisibility(string questName, int objectiveIndex, ObjectiveStatus status)
    {
        var quest = GetQuest(questName);
        if (quest != null && objectiveIndex >= 0 && objectiveIndex < quest.objectives.Count)
        {
            quest.objectives[objectiveIndex].status = status;
            UpdateObjectiveText(quest);
        }
    }
}
